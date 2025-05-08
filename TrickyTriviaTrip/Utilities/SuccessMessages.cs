namespace TrickyTriviaTrip.Utilities
{
    /// <summary>
    /// Provides randomized success messages
    /// </summary>
    public static class SuccessMessages
    {   
        private static readonly string[] Texts = 
        {
            "Good job!", "Excellent!", "Well done!", "Nice work!", "Nailed it!",
            "You got it!", "Right on!", "Perfect!", "Bravo!", "Fantastic!",
            "Great job!", "Super!", "You rock!", "Incredible!", "Amazing!",
            "Sweet!", "Genius!", "That's it!", "Brilliant!", "Sharp thinking!",
            "That's right!", "So smart!", "You did it!", "Awesome!", "Keep it up!",
            "Success!", "Nicely done!", "Good thinking!", "Clever!", "You're on fire!",
            "Legendary!", "Epic!", "Correct!", "Bullseye!", "Yes!",
            "Magnificent!", "Masterful!", "Big brain!", "Smooth!", "Smashing!",
            "High five!", "Top notch!", "A+ work!", "Right answer!", "Marvellous!",
            "Spectacular!", "Phenomenal!", "Extraordinary!", "Wonderful!", "Astounding!",
            "Impressive!", "Outstanding!", "Flawless!", "Exactly!", "Spot on!",
            "Terrific!", "Fabulous!", "Stellar!", "Exceptional!", "Splendid!",
            "Cool!", "Monumental!", "Expert class!", "Formidable!", "Amazeballs!"
        };

        private static readonly string[] Emojis =
        {
            "👍👍👍", "👏👏👏", "✨👍✨", "💯👍💯", "🎉👍🎉", "🚀👍🚀", "🔥🔥🔥", 
            "👊👊👊", "💥💥💥", "🏆🏆🏆", "😎😎😎", "👏🎉👏", "🌟🌟🌟", "🥇🥇🥇"
        };

        private static Random rnd = new Random();

        /// <summary>
        /// Provides a random text message out of the pool
        /// </summary>
        public static string Text => Texts[rnd.Next(Texts.Length)];

        /// <summary>
        /// Provides a random emoji message out of the pool
        /// </summary>
        public static string Emoji => Emojis[rnd.Next(Emojis.Length)];



        /// <summary>
        /// (For debugging)
        /// Returns all emoji strings
        /// (to check how they are displayed, for example, after a font change)
        /// </summary>
        public static string TestAllEmojis => string.Join(" ", Emojis);

    }
}
