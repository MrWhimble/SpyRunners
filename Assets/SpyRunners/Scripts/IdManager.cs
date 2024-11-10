namespace SpyRunners
{
    public static class IdManager
    {
        private static int _lastId = 0;
        public static int GetId() => _lastId++;
    }
}