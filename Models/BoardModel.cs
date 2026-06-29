using System.Text.Json;

namespace CST_350_Milestone.Models
{
    public class BoardModel
    {
        public List<CellModel> Cells { get; set; } = new();
        public int Size { get; set; }
        public string Difficulty { get; set; } = "";
        public int TotalMines { get; set; }
        public bool GameOver { get; set; }
        public bool GameWon { get; set; }
        public bool FirstClick { get; set; } = true;
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public int FinalElapsedSeconds { get; set; }

        public int GetElapsedSeconds() =>
            StartTime == DateTime.MinValue ? 0 :
            GameOver ? FinalElapsedSeconds :
            (int)(DateTime.UtcNow - StartTime).TotalSeconds;

        public int RevealedCellCount => Cells.Count(c => c.CellState == 1);

        public int CalculateScore()
        {
            double multiplier = Difficulty switch
            {
                "Easy"   => 1.0,
                "Medium" => 1.5,
                "Hard"   => 2.0,
                _        => 1.0
            };
            return Math.Max(0, (int)(TotalMines * 100 * multiplier) - FinalElapsedSeconds * 5);
        }

        public BoardModel() { }

        public BoardModel(int size, string difficulty)
        {
            Size = size;
            Difficulty = difficulty;
            TotalMines = CalculateMineCount(size, difficulty);

            for (int i = 0; i < size * size; i++)
                Cells.Add(new CellModel(i, 0, "blue_button.png"));
        }

        private static int CalculateMineCount(int size, string difficulty) => difficulty switch
        {
            "Easy"   => (int)Math.Round(size * size * 0.12),
            "Medium" => (int)Math.Round(size * size * 0.17),
            "Hard"   => (int)Math.Round(size * size * 0.23),
            _        => (int)Math.Round(size * size * 0.15)
        };

        public void PlaceMines(int safeCell)
        {
            var rand = new Random();
            int placed = 0;
            while (placed < TotalMines)
            {
                int idx = rand.Next(Cells.Count);
                if (!Cells[idx].IsMine && idx != safeCell)
                {
                    Cells[idx].IsMine = true;
                    placed++;
                }
            }

            for (int i = 0; i < Cells.Count; i++)
                if (!Cells[i].IsMine)
                    Cells[i].AdjacentMines = CountAdjacentMines(i);
        }

        private int CountAdjacentMines(int index)
        {
            int count = 0;
            foreach (int n in GetNeighbors(index))
                if (Cells[n].IsMine) count++;
            return count;
        }

        private List<int> GetNeighbors(int index)
        {
            var neighbors = new List<int>();
            int row = index / Size, col = index % Size;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < Size && nc >= 0 && nc < Size)
                        neighbors.Add(nr * Size + nc);
                }
            return neighbors;
        }

        public void RevealCell(int index)
        {
            if (index < 0 || index >= Cells.Count) return;

            if (FirstClick)
            {
                PlaceMines(index);
                StartTime = DateTime.UtcNow;
                FirstClick = false;
            }

            var cell = Cells[index];
            if (cell.CellState != 0) return;

            if (cell.IsMine)
            {
                FinalElapsedSeconds = GetElapsedSeconds();
                GameOver = true;
                foreach (var c in Cells.Where(c => c.IsMine))
                {
                    c.CellState = 3;
                    c.CellImage = "red_button.png";
                }
                return;
            }

            FloodReveal(index);

            if (Cells.All(c => c.IsMine || c.CellState == 1))
            {
                FinalElapsedSeconds = GetElapsedSeconds();
                GameWon = true;
                GameOver = true;
            }
        }

        private void FloodReveal(int index)
        {
            var queue = new Queue<int>();
            var visited = new HashSet<int>();
            queue.Enqueue(index);

            while (queue.Count > 0)
            {
                int idx = queue.Dequeue();
                if (!visited.Add(idx)) continue;

                var cell = Cells[idx];
                if (cell.CellState != 0 || cell.IsMine) continue;

                cell.CellState = 1;
                cell.CellImage = cell.AdjacentMines == 0 ? "green_button.png" : "orange_button.png";

                if (cell.AdjacentMines == 0)
                    foreach (int n in GetNeighbors(idx))
                        if (!visited.Contains(n)) queue.Enqueue(n);
            }
        }

        public string ToJson() => JsonSerializer.Serialize(this);
        public static BoardModel? FromJson(string json) => JsonSerializer.Deserialize<BoardModel>(json);
    }
}
