namespace ConceptualComponentConverter
{
    public class MockTekla : ITekla
    {
        public bool IsAnyConnectionSelected()
        {
            return true;
        }

        public bool IsRunning()
        {
            return true;
        }
    }
}
