using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SayedHa.Blackjack.Shared.Blackjack {
    public class SessionReportData {
        public SessionReportData(Participant player) { 
            Player = player;
        }
        public Participant Player { get; protected init; }

        public Dictionary<string, int> WrongNextActionAndCount { get; protected set; } = new Dictionary<string, int>();

        public void AddWrongNextActionSelected(HandAction nextActionSelected, HandActionAndReason correctAction) {
            int numTimesEncountered = int.MinValue;
            WrongNextActionAndCount.TryGetValue(correctAction.Reason, out numTimesEncountered);
            if (numTimesEncountered < 0) {
                numTimesEncountered = 0;
            }

            numTimesEncountered++;

            WrongNextActionAndCount[correctAction.Reason] = numTimesEncountered;
        }
    }
}
