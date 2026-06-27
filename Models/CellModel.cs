using System.ComponentModel.DataAnnotations;

namespace CST_350_Milestone.Models
{
    public class CellModel
    {
        // Cell state: 0=hidden, 1=revealed, 2=flagged, 3=mine, 4-11=numbers 1-8
        public int Id { get; set; }
        public int CellState { get; set; }
        public string CellImage { get; set; }

        public CellModel(int id, int cellState, string cellImage)
        {
            Id = id;
            CellState = cellState;
            CellImage = cellImage;
        }

        public CellModel() { }
    }
}
