using System;

namespace BlackjackBacktest
{
    public class Card
    {
        public enum Suits
        {
            Heart,
            Diamond,
            Club,
            Spade
        }

        public enum Faces
        {
            Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
        }

        public bool IsFaceUp;
        public byte Suit;
        public byte Face;

        // Suits are Heart = 0, Diamond, Club, Spade
        // Faces are Ace = 1, Two, Three, ... Jack, Queen, King

        public Card(byte suit, byte face)
        {
            Suit = suit;
            Face = face;
            IsFaceUp = true;  // default
        }

        public Card(byte suit, byte face, bool faceUp)
        {
            Suit = suit;
            Face = face;
            IsFaceUp = faceUp;
        }
    }
}
