import subprocess
import sys

def run_triangle_script(a, b, c):
    result = subprocess.run(
        ["python", "main.py", a, b, c],
        capture_output=True,
        text=True,
        check=True
    )
    return result.stdout.strip()


def test_triangle(test_cases_file, output_file):
    with open(test_cases_file, "r", encoding="utf-8") as infile, open(output_file, "w", encoding="utf-8") as outfile:
        for line in infile:
            parts = line.strip().split()
            if len(parts) < 4:
                outfile.write("error;\n")
                continue

            a, b, c, expected = parts[0], parts[1], parts[2], " ".join(parts[3:])

            actual = run_triangle_script(a, b, c)

            if actual == expected:
                outfile.write("success;\n")
            else:
                outfile.write(f"error;\n")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print(" python tests.py <тесты> <рез-ты>")
    else:
        test_cases_file = sys.argv[1]
        output_file = sys.argv[2]
        test_triangle(test_cases_file, output_file)