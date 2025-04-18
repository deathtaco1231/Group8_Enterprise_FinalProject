﻿using Group8_Enterprise_FinalProject.Entities;

namespace Group8_Enterprise_FinalProject.Models
{
    public class GameViewModel
    {
        public Game? ActiveGame { get; set; } = new Game
        {
            GameDateTime = DateTime.Now
        };

        public string? WinningTeamName { get; set; }
    }
}
