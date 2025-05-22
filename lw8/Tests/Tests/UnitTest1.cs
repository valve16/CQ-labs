using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace ShopApiTests
{
    public class Product
    {
        public int? id { get; set; }
        public int category_id { get; set; }
        public string title { get; set; }
        public string alias { get; set; }
        public string content { get; set; }
        public double price { get; set; } 
        public double old_price { get; set; }
        public int status { get; set; }
        public string keywords { get; set; }
        public string description { get; set; }
        public int hit { get; set; }
    }

    public class ProductApiTests : IAsyncDisposable
    {
        private const string BASE_URL = "http://shop.qatl.ru";
        private readonly HttpClient client;
        private readonly ITestOutputHelper output;

        public ProductApiTests(ITestOutputHelper output)
        {
            this.output = output;
            client = new HttpClient();
        }

        [Theory]
        [MemberData(nameof(GetAddValidProductTestData))]
        public async Task AddValidProductTest(Product product)
        {
            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{BASE_URL}/api/addproduct", content);
            var responseContent = await response.Content.ReadAsStringAsync();
   
            Assert.True(response.IsSuccessStatusCode, "Ожидался успешный статус ответа");
            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
            int id = responseJson.id;

            //// Проверяем, что товар добавлен
            var productsResponse = await client.GetAsync($"{BASE_URL}/api/products");
            var productsContent = await productsResponse.Content.ReadAsStringAsync();
            //output.WriteLine($"Ответ GET /api/products: {productsContent}"); // Логируем для отладки

            List<Product> products;
            try
            {
                products = JsonConvert.DeserializeObject<List<Product>>(productsContent);
            }
            catch (JsonReaderException ex)
            {
                output.WriteLine($"Ошибка десериализации: {ex.Message}");
                throw; 
            }

            var addedProduct = products.FirstOrDefault(p => p.id == id);
            Assert.True(addedProduct != null, "Товар не найден в списке");
            AssertProductEquals(product, addedProduct);

            await client.GetAsync($"{BASE_URL}/api/deleteproduct?id={id}");
            
        }

        [Theory]
        [MemberData(nameof(GetAddInvalidProductTestData))]
        public async Task AddInValidProductTest(Product product)
        {
            var content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{BASE_URL}/api/addproduct", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode, $"Failed to add product. Status Code: {response.StatusCode}");

            var responseJson = JsonConvert.DeserializeObject<dynamic>(responseContent);
            int id = responseJson.id;
            Assert.NotNull(responseJson);

            var productsResponse = await client.GetAsync($"{BASE_URL}/api/products");
            var productsContent = await productsResponse.Content.ReadAsStringAsync();

            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productsContent);

            var addedProduct = products.FirstOrDefault(p => p.id == id);
            Assert.False(addedProduct != null, "Некорректный товар найден в списке");
        }



        [Fact]
        public async Task DeleteExistingProductTest()
        {
            var testProduct = new Product
            {
                category_id = 1,
                title = "товар для удаления",
                content = "товар для удаления",
                price = 100.0,
                old_price = 120.0,
                status = 1,
                keywords = "товар для удаления",
                description = "товар для удаления",
                hit = 0
            };

            var content = new StringContent(JsonConvert.SerializeObject(testProduct), Encoding.UTF8, "application/json");
            var addResponse = await client.PostAsync($"{BASE_URL}/api/addproduct", content);
            var addResponseContent = await addResponse.Content.ReadAsStringAsync();

            Assert.True(addResponse.IsSuccessStatusCode, "Не удалось добавить товар для теста удаления");

            var addedProduct = JsonConvert.DeserializeObject<Product>(addResponseContent);
            Assert.NotNull(addedProduct?.id);

            var deleteResponse = await client.DeleteAsync($"{BASE_URL}/api/deleteproduct?id={addedProduct.id}");
            Assert.True(deleteResponse.IsSuccessStatusCode, $"Удаление не удалось. Status: {deleteResponse.StatusCode}");

            var getResponse = await client.GetAsync($"{BASE_URL}/api/products");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(getContent);

            var exists = products.Any(p => p.id == addedProduct.id);
            Assert.False(exists, "Удаленный товар все еще присутствует в списке");
        }

        [Fact]
        public async Task DeleteNonExistingProductTest()
        {
            int nonExistentId = -99999;

            var deleteResponse = await client.DeleteAsync($"{BASE_URL}/api/products/{nonExistentId}");

            // Проверяем, что сервер корректно обработал попытку удалить несуществующий товар
            Assert.False(deleteResponse.IsSuccessStatusCode, "Удаление несуществующего товара прошло успешно, что неверно.");

            var content = await deleteResponse.Content.ReadAsStringAsync();
            output.WriteLine($"Ответ при удалении несуществующего товара: {content}");

            Assert.True(deleteResponse.StatusCode == HttpStatusCode.NotFound ||
                        deleteResponse.StatusCode == HttpStatusCode.BadRequest,
                        $"Ожидался 404 или 400, получено: {deleteResponse.StatusCode}");
        }

        [Fact]
        public async Task EditExistingProductTest()
        {
            var product = new Product
            {
                category_id = 1,
                title = "Оригинальный товар",
                alias = "original-product",
                content = "Описание",
                price = 100.0,
                old_price = 120.0,
                status = 1,
                keywords = "оригинал",
                description = "Тест",
                hit = 0
            };

            var addContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var addResponse = await client.PostAsync($"{BASE_URL}/api/addproduct", addContent);
            var addResponseContent = await addResponse.Content.ReadAsStringAsync();

            Assert.True(addResponse.IsSuccessStatusCode,
                $"Не удалось добавить товар. Status: {addResponse.StatusCode}, Response: {addResponseContent}");

            var addedProduct = JsonConvert.DeserializeObject<Product>(addResponseContent);
            output.WriteLine($"ID добавленного товара: {addedProduct?.id}");

            var updatedProduct = new Product
            {
                id = addedProduct.id,
                category_id = addedProduct.category_id,
                title = "Обновленный товар",
                alias = "updated-product",
                content = "Обновленное описание",
                price = 200.0,
                old_price = 150.0,
                status = 1,
                keywords = "обновленный",
                description = "Обновленный тест",
                hit = 1
            };

            var editContent = new StringContent(JsonConvert.SerializeObject(updatedProduct), Encoding.UTF8, "application/json");
            var editResponse = await client.PostAsync($"{BASE_URL}/api/editproduct", editContent);
            var editResponseContent = await editResponse.Content.ReadAsStringAsync();

            Assert.True(editResponse.IsSuccessStatusCode,
                $"Редактирование не удалось. Status: {editResponse.StatusCode}, Response: {editResponseContent}");

            var getResponse = await client.GetAsync($"{BASE_URL}/api/products");
            var getResponseContent = await getResponse.Content.ReadAsStringAsync();

            Assert.True(getResponse.IsSuccessStatusCode,
                $"Не удалось получить товар. Status: {getResponse.StatusCode}");

            getResponse = await client.GetAsync($"{BASE_URL}/api/products");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(getContent);

            var actualProduct = products.FirstOrDefault(p => p.id == addedProduct.id);

            AssertProductEquals(updatedProduct, actualProduct);

            var deleteResponse = await client.GetAsync($"{BASE_URL}/api/deleteproduct?id={updatedProduct.id}");
        }

        [Fact]
        public async Task EditNonExistingProductTest()
        {
            // Подготовим несуществующий продукт с фейковым ID
            var nonExistentProduct = new Product
            {
                id = -999999,
                category_id = 1,
                title = "Несуществующий",
                alias = "no-such-product",
                content = "Нет такого",
                price = 50.0,
                old_price = 60.0,
                status = 1,
                keywords = "фейк",
                description = "ничего",
                hit = 0
            };

            var content = new StringContent(JsonConvert.SerializeObject(nonExistentProduct), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{BASE_URL}/api/editproduct", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.True(response.IsSuccessStatusCode, $"Failed to edit product. Status Code: {response.StatusCode}");

            output.WriteLine($"Ответ при редактировании несуществующего товара: {responseContent}");

            var getResponse = await client.GetAsync($"{BASE_URL}/api/products");
            var getContent = await getResponse.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Product>>(getContent);

            var exists = products.Any(p => p.id == nonExistentProduct.id);
            Assert.False(exists, "Редактировался некорректный товар с id");
        }

        private void AssertProductEquals(Product expected, Product actual)
        {
            Assert.Equal(expected.category_id, actual.category_id);
            Assert.Equal(expected.title, actual.title);
            Assert.Equal(expected.content, actual.content);
            Assert.Equal(expected.price, actual.price);
            Assert.Equal(expected.old_price, actual.old_price);
            Assert.Equal(expected.status, actual.status);
            Assert.Equal(expected.keywords, actual.keywords);
            Assert.Equal(expected.description, actual.description);
            Assert.Equal(expected.hit, actual.hit);
            // Поле alias не проверяем, так как оно генерируется сервером
        }

        public static IEnumerable<object[]> GetAddValidProductTestData()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../addtestdata.json");
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"JSON-фийл с тестовыми данными не найден: {jsonFilePath}");
            }

            string jsonData = File.ReadAllText(jsonFilePath);
            var testData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);

            // Определяем, какие тесты ожидаются успешными
            var validKeys = new List<string> { "valid", "valid_rus" };

            foreach (var testCase in testData)
            {
                if (!validKeys.Contains(testCase.Key))
                    continue;

                var product = new Product
                {
                    category_id = int.Parse(testCase.Value["category_id"]),
                    title = testCase.Value["title"],
                    content = testCase.Value["content"],
                    price = double.Parse(testCase.Value["price"]),
                    old_price = double.Parse(testCase.Value["old_price"]),
                    status = int.Parse(testCase.Value["status"]),
                    keywords = testCase.Value["keywords"],
                    description = testCase.Value["description"],
                    hit = int.Parse(testCase.Value["hit"])
                };

                yield return new object[] { product };
            }
        }
        public static IEnumerable<object[]> GetAddInvalidProductTestData()
        {
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../addtestdata.json");
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"JSON-фийл с тестовыми данными не найден: {jsonFilePath}");
            }

            string jsonData = File.ReadAllText(jsonFilePath);
            var testData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);

            var validKeys = new List<string> { "invalid_category_id_less", "invalid_category_id_more", "invalid_status",
                "invalid_status_more", "invalid_hit",  "invalid_hit_more" };

            foreach (var testCase in testData)
            {
                if (!validKeys.Contains(testCase.Key))
                    continue;

                var product = new Product
                {
                    category_id = int.Parse(testCase.Value["category_id"]),
                    title = testCase.Value["title"],
                    content = testCase.Value["content"],
                    price = double.Parse(testCase.Value["price"]),
                    old_price = double.Parse(testCase.Value["old_price"]),
                    status = int.Parse(testCase.Value["status"]),
                    keywords = testCase.Value["keywords"],
                    description = testCase.Value["description"],
                    hit = int.Parse(testCase.Value["hit"])
                };

                yield return new object[] { product };
            }
        }

        
        public async ValueTask DisposeAsync()
        {
            client.Dispose();
        }
    }
}