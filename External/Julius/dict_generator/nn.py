with open("./grammar_after.txt", "w", encoding="shift-jis") as f:
    for i in range(285):
        f.write("OTHER\t: L" + str(i) + "\n")
