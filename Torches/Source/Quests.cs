using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches
{
    public enum QuestStatus
    {
        Available,
        Started,
        Completed
    }

    public static class Quests
    {
        public static QuestStatus TribesmanQuest { get; set; } = QuestStatus.Available;
        public static QuestStatus HermitQuest { get; set; } = QuestStatus.Available;
        //public static QuestStatus MazeQuest { get; set; } = QuestStatus.Available;
    }
}
