﻿using SayedHa.Blackjack.Shared.Blackjack.Exceptions;

namespace SayedHa.Blackjack.Shared.Blackjack.Strategy {
    public class NextHandActionConverter {
        private NextHandActionConverter() { }
        public static NextHandActionConverter Instance { get; } = new NextHandActionConverter();
        protected internal HandAction? GetHandActionFor(int handActionInt) => handActionInt switch {
            0 => null,
            1 => HandAction.Double,
            2 => HandAction.Hit,
            3 => HandAction.Stand,
            4 => HandAction.Split,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for handActionInt: '{handActionInt}'")
        };
        public int ConvertBoolToInt(bool value) => value switch {
            true => 1,
            false => 2
        };
        public bool? ConvertBoolIndex(int intValue) => intValue switch {
            1 => true,
            2 => false,
            _ => null
        };
        public char GetCharForBoolIndex(int boolIndex) => boolIndex switch {
            1 => 'Y',
            2 => 'N',
            _ => 'X'
        };
        public char GetCharForHandActionIndex(int haIndex) => haIndex switch {
            1 => 'D',
            2 => 'H',
            3 => 'S',
            4 => 'x',
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for haIndex: '{haIndex}'")
        };
        public int GetIntFor(bool boolValue) => boolValue switch {
            // not using 0 because that's the default value that the array is initilized with
            true => 1,
            false => 2
        };
        public int GetIntFor(CardNumber cardNumber) => cardNumber switch {
            CardNumber.Two => 0,
            CardNumber.Three => 1,
            CardNumber.Four => 2,
            CardNumber.Five => 3,
            CardNumber.Six => 4,
            CardNumber.Seven => 5,
            CardNumber.Eight => 6,
            CardNumber.Nine => 7,
            CardNumber.Ten => 8,
            CardNumber.Jack => 8,
            CardNumber.Queen => 8,
            CardNumber.King => 8,
            CardNumber.Ace => 9,
            _ => throw new UnknownValueException($"Unknown value for CardNumber: '{cardNumber}'")
        };
        /// <summary>
        /// This is the opposite of  <code>GetIntFor(CardNumber)</code>.
        /// </summary>
        /// <param name="splitIndex"></param>
        /// <returns></returns>
        public string GetSplitCharForIndex(int splitIndex) => splitIndex switch {
            0 => "2",
            1 => "3",
            2 => "4",
            3 => "5",
            4 => "6",
            5 => "7",
            6 => "8",
            7 => "9",
            8 => "10",
            9 => "A",
            _ => throw new UnknownValueException($"Unknown value for split index: '{splitIndex}'")
        };
        public int GetIntForSoftScore(int softScore) => softScore switch {
            2 => 0,
            3 => 1,
            4 => 2,
            5 => 3,
            6 => 4,
            7 => 5,
            8 => 6,
            9 => 7,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for soft total: '{softScore}'")
        };
        public int GetSoftTotalScoreFromIndex(int softTotalIndex) => softTotalIndex switch {
            0 => 2,
            1 => 3,
            2 => 4,
            3 => 5,
            4 => 6,
            5 => 7,
            6 => 8,
            7 => 9,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for soft total index: '{softTotalIndex}'")
        };
        public int GetIntHardTotalCellValueFromScore(int hardTotalScore) => hardTotalScore switch {
            3  => 0,
            4  => 1,
            5  => 2,
            6  => 3,
            7  => 4,
            8  => 5,
            9  => 6,
            10 => 7,
            11 => 8,
            12 => 9,
            13 => 10,
            14 => 11,
            15 => 12,
            16 => 13,
            17 => 14,
            18 => 15,
            19 => 16,
            20 => 17,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for hard total: '{hardTotalScore}'")
        };
        /// <summary>
        /// This does the opposite of <code>GetIntHardTotalCellValueFromScore</code>
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        /// <exception cref="UnexpectedNodeTypeException"></exception>
        public int GetHardTotalScoreFromIndex(int cellValue) => cellValue switch {
            0 => 3,
            1 => 4,
            2 => 5,
            3 => 6,
            4 => 7,
            5 => 8,
            6 => 9,
            7 => 10,
            8 => 11,
            9 => 12,
            10 => 13,
            11 => 14,
            12 => 15,
            13 => 16,
            14 => 17,
            15 => 18,
            16 => 19,
            17 => 20,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for hard total score: '{cellValue}'")
        };
        public int GetIntFor(HandAction handAction) => handAction switch {
            HandAction.Double => 1,
            HandAction.Hit => 2,
            HandAction.Stand => 3,
            HandAction.Split => 4,
            _ => throw new UnexpectedNodeTypeException($"Unexpected value for HandAction: '{handAction}'")
        };
    }
}
