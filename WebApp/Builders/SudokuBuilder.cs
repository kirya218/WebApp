namespace WebApp.Builders
{
    public class SudokuBuilder
    {
        /// <summary>
        /// Размер блока.
        /// </summary>
        private readonly int _sizeBlock;

        public SudokuBuilder(int sizeBlock)
        {
            _sizeBlock = sizeBlock;
        }

        /// <summary>
        /// Генератор судоку.
        /// </summary>
        /// <returns>Сгенерированный судоку.</returns>
        public (int[][], int[,]) GenerateSudoku(Action<int[][]> action = null)
        {
            var sudoku = GenarateBaseSudoku();
            SwapColumns(sudoku);
            SwapColumns(sudoku);
            SwapColumnsArea(sudoku);
            SwapRows(sudoku);
            SwapRowsArea(sudoku);
            SwapColumnsArea(sudoku);
            SwapRows(sudoku);
            SwapColumns(sudoku);

            action?.Invoke(sudoku);

            var sudokuWithHideElement = HideElements(sudoku);

            return (sudoku, sudokuWithHideElement);
        }

        /// <summary>
        /// Генерирует базовый судоку.
        /// </summary>
        /// <returns>Базовый судоку.</returns>
        private int[][] GenarateBaseSudoku()
        {
            var sudoku = new List<int[]>();

            for (var i = 0; i < _sizeBlock; i++)
            {
                var startCell = i + 1;

                for (var j = 0; j < _sizeBlock; j++)
                {
                    var list = new List<int>();
                    var currentCell = startCell;

                    for (var z = 0; z < _sizeBlock * _sizeBlock; z++)
                    {
                        if (currentCell > _sizeBlock * _sizeBlock)
                        {
                            currentCell = 1;
                        }

                        list.Add(currentCell++);
                    }
                    startCell += _sizeBlock;
                    sudoku.Add(list.ToArray());
                }
            }

            return sudoku.ToArray();
        }

        /// <summary>
        /// Транспонирует строки в колонку.
        /// </summary>
        /// <param name="arr">Исходные данные.</param>
        private static void TransposeRowsAndColumns(int[][] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    (arr[i][j], arr[j][i]) = (arr[j][i], arr[i][j]);
                }
            }
        }

        /// <summary>
        /// Свапает строки.
        /// </summary>
        /// <param name="sudoku">Исходный судоку.</param>
        private void SwapRows(int[][] sudoku)
        {
            var random = new Random();
            var numberArea = random.Next(0, _sizeBlock);
            var numberFirstRowInArea = random.Next(0, _sizeBlock - 1);
            var numberSecondRowInArea = random.Next(0, _sizeBlock - 1);

            while (numberFirstRowInArea == numberSecondRowInArea)
            {
                numberSecondRowInArea = random.Next(0, _sizeBlock - 1);
            }

            var numberFirstRow = numberArea * _sizeBlock + numberFirstRowInArea;
            var numberSecondRow = numberArea * _sizeBlock + numberSecondRowInArea;

            (sudoku[numberFirstRow], sudoku[numberSecondRow]) = (sudoku[numberSecondRow], sudoku[numberFirstRow]);
        }

        /// <summary>
        /// Свапает колонки.
        /// </summary>
        /// <param name="sudoku">Исходный судоку.</param>
        private void SwapColumns(int[][] sudoku)
        {
            TransposeRowsAndColumns(sudoku);
            SwapRows(sudoku);
            TransposeRowsAndColumns(sudoku);
        }

        /// <summary>
        /// Свапает строны строк.
        /// </summary>
        /// <param name="sudoku">Исходный судоку.</param>
        private void SwapRowsArea(int[][] sudoku)
        {
            var random = new Random();
            var numberFirstArea = random.Next(0, _sizeBlock);
            var numberSecondArea = random.Next(0, _sizeBlock);

            while (numberFirstArea == numberSecondArea)
            {
                numberSecondArea = random.Next(0, _sizeBlock);
            }

            for (var i = 0; i < _sizeBlock; i++)
            {
                (var numberFirstAreaRow, var numberSecondAreaRow) = (numberFirstArea * _sizeBlock + i, numberSecondArea * _sizeBlock + i);
                (sudoku[numberFirstAreaRow], sudoku[numberSecondAreaRow]) = (sudoku[numberSecondAreaRow], sudoku[numberFirstAreaRow]);
            }
        }

        /// <summary>
        /// Свапает стороны колонок.
        /// </summary>
        /// <param name="sudoku">Исходный судоку.</param>
        private void SwapColumnsArea(int[][] sudoku)
        {
            TransposeRowsAndColumns(sudoku);
            SwapRowsArea(sudoku);
            TransposeRowsAndColumns(sudoku);
        }

        private int[,] HideElements(int[][] sudoku)
        {
            var random = new Random();
            var viewed = new bool[_sizeBlock * _sizeBlock, _sizeBlock * _sizeBlock];
            var sudokuWithHidedElement = To2D(sudoku, _sizeBlock * _sizeBlock);


            var iterator = 0;
            var difficult = Math.Pow(_sizeBlock, 4);

            while (iterator < Math.Pow(_sizeBlock, 4))
            {
                (var row, var column) = (random.Next(0, _sizeBlock * _sizeBlock), random.Next(0, _sizeBlock * _sizeBlock));

                if (viewed[row, column] == true)
                {
                    continue;
                }

                iterator++;
                viewed[row, column] = true;

                var temp = sudokuWithHidedElement[row, column];
                sudokuWithHidedElement[row, column] = 0;
                difficult--;

                var line = GetRowFixedNumbers(row, sudokuWithHidedElement);
                var col = GetColumnFixedNumbers(column, sudokuWithHidedElement);
                var block = GetBlockFixedNumbers(row, column, sudokuWithHidedElement);

                var variants = Enumerable.Range(1, 9).Except(line).Except(block).Except(col);

                if (variants.Count() != 1)
                {
                    sudokuWithHidedElement[row, column] = temp;
                    difficult++;
                }
            }

            return sudokuWithHidedElement;
        }

        private int[,] To2D(int[][] source, int _sizeBlock)
        {
            var result = new int[_sizeBlock, _sizeBlock];
            for (int i = 0; i < _sizeBlock; ++i)
                for (int j = 0; j < _sizeBlock; ++j)
                    result[i, j] = source[i][j];

            return result;
        }

        private IEnumerable<int> GetRowFixedNumbers(int rowIndex, int[,] sudoku)
        {
            for (int x = 0; x < 9; x++)
            {
                if (sudoku[rowIndex, x] != 0) yield return sudoku[rowIndex, x];
            }
        }
        private IEnumerable<int> GetColumnFixedNumbers(int columnIndex, int[,] sudoku)
        {
            for (int y = 0; y < 9; y++)
            {
                if (sudoku[y, columnIndex] != 0) yield return sudoku[y, columnIndex];
            }
        }
        private IEnumerable<int> GetBlockFixedNumbers(int rowIndex, int columnIndex, int[,] sudoku)
        {
            int startX = 3 * (rowIndex / 3);
            int startY = 3 * (columnIndex / 3);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (sudoku[i + startX, j + startY] != 0)
                        yield return sudoku[i + startX, j + startY];
                }
            }
        }
    }
}
