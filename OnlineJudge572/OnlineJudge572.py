class Pocket:
    def __init__(self, x, y):
        self.x = x
        self.y = y

class Deposit:
    def __init__(self):
        self.pockets = []

class Grid:
    def __init__(self):
        self.deposits = []

def visita(i, j, matriz, m, n):
    matriz[i][j] = False

    deposit = Deposit()
    pockets = []
    pocket = Pocket(i, j)
    pockets.append(pocket)

    acima = i - 1
    abaixo = i + 1
    esq = j - 1
    dir = j + 1

    for ver in range(i - 1, i + 1):
        if ver != i and ver >= 0 and ver < m:
            for hor in range(j - 1, j + 1):
                if hor != j and hor >= 0 and ver < n:
                    if matriz[ver][hor] == True:
                        pcs = visita(ver, hor, matriz, m, n)
                        for p in pcs:
                            pockets.append(p)
    return pockets

grid = input().split(" ")

linhas = int(grid[0])
cols = int(grid[1])

matriz = []

for linha in range(linhas):
    row = []
    i = input()
    for char in i:
        if char == "*":
            row.append(False)
        else:
            row.append(True)
    matriz.append(row)

grid = Grid()

for i in range(linhas):
    for j in range(cols):
        if matriz[i][j] == True:
            pockets = visita(i, j, matriz, linhas, cols)
            grid.deposits.append(pockets)

print(grid.deposits[0])