namespace SayedHa.Blackjack.Shared.Roulette {
    public class EnumHelper {
        private EnumHelper() {
            _allGameCellGroup = ((GameCellGroup[])Enum.GetValues(typeof(GameCellGroup))).ToArray();
        }

        public GameCellGroup[] GetAllGameCellGroup() => _allGameCellGroup;

        private static EnumHelper _instance = new EnumHelper();
        private readonly GameCellGroup[] _allGameCellGroup;

        public static EnumHelper GetHelper() => _instance;
    }
}
