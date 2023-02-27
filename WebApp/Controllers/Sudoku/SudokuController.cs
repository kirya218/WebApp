using Microsoft.AspNetCore.Mvc;
using WebApp.Builders;
using WebApp.Models.Sudoku;

namespace WebApp.Controllers.Sudoku
{
    public class SudokuController : Controller
    {
        public ActionResult Index()
        {
            var builderSudoku = new SudokuBuilder(3);
            (var sudoku, var sudokuHided) = builderSudoku.GenerateSudoku();

            var model = new SudokuViewModel()
            {
                Sudoku = sudoku,
                SudokuHided = sudokuHided
            };

            return View(model);
        }
    }
}
