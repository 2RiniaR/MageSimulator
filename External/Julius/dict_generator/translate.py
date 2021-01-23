import subprocess
from yomi2voca import yomi2voca


# 辞書ファイルの出力先ディレクトリ
OUTPUT_DIR_NAME = "dist"

def generate_files(output_file_name="gram"):
    output_file_path = OUTPUT_DIR_NAME + "\\" + output_file_name
    yomi2voca(output_file_path)
    subprocess.run(["python", "mkdfa.py", output_file_path])


if __name__ == "__main__":
    generate_files("alltest_add")
