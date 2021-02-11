import subprocess
from yomi2voca import yomi2voca


# 辞書ファイルの出力先ディレクトリ
OUTPUT_DIR_NAME = "dist"


def generate_files(phrase, output_file_name="gram"):
    output_file_path = OUTPUT_DIR_NAME + "\\" + output_file_name
    output_yomi_file_path = output_file_path + ".yomi"
    output_grammar_file_path = output_file_path + ".grammar"

    # .yomi ファイルの生成
    with open(output_yomi_file_path, "w", encoding="utf-8") as f:
        for i, word in enumerate(phrase):
            f.write("% W" + str(i) + "\n")
            f.write(word["text"] + " " + word["sound"] + "\n")

    # .grammer ファイルの生成
    with open(output_grammar_file_path, "w", encoding="utf-8") as f:
        f.write("S  :  NS_B")
        for i in range(len(phrase)):
            f.write(" W" + str(i))
        f.write(" NS_E")

    # .yomi ファイルを .voca ファイルに変換
    yomi2voca(output_file_path)
    # その他julius起動に必要な辞書ファイルを生成
    subprocess.run(["python", "mkdfa.py", output_file_path])


if __name__ == "__main__":

    # 入力する配列のサンプル
    chant1 = [
        { "text": "ファイア", "sound": "ふぁいあ" },
        { "text": "ブラスト", "sound": "ぶらすと" },
    ]

    chant2 = [
        { "text": "レーザー", "sound": "れーざー" },
        { "text": "ビーム", "sound": "びーむ" },
    ]

    chant3 = [
        { "text": "ソウル", "sound": "そーる" },
        { "text": "カッター", "sound": "かったー" },
    ]

    chant4 = [
        { "text": "メテオ", "sound": "めてお" },
        { "text": "レイン", "sound": "れいん" },
    ]

    # generate_files(chant1, "chant1")
    # generate_files(chant2, "chant2")
    generate_files(chant3, "chant3")
    # generate_files(chant4, "chant4")
