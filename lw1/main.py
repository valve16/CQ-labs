import sys

def determine_triangle_type(a, b, c):
    try:
        a = float(a)
        b = float(b)
        c = float(c)

        if a <= 0 or b <= 0 or c <= 0:
            return "не_треугольник"

        if a + b <= c or a + c <= b or b + c <= a:
            return "не_треугольник"

        if a == b == c:
            return "равносторонний"
        elif a == b or a == c or b == c:
            return "равнобедренный"
        else:
            return "обычный"
    except:
        return "неизвестная_ошибка"


if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("3_аргумента!")
    else:
        a, b, c = sys.argv[1], sys.argv[2], sys.argv[3]
        result = determine_triangle_type(a, b, c)
        print(result)