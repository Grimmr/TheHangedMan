namespace AIBase.Enums
{
    public enum Player
    {
        Bot,
        Enemy
    }

    public static class PlayerExtentions
    {
        public static Player Not(this Player p)
        {
            switch(p)
            {
                case Player.Bot: return Player.Enemy;
                case Player.Enemy: return Player.Bot;
            }

            //dummy ret
            return Player.Bot;
        }
    }

}