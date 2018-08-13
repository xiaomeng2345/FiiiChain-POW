namespace FiiiChain.Consensus
{
    public class BlockSetting
    {
        //Max block size is 3 MB
        public const long MAX_BLOCK_SIZE = 3 * 1024 * 1024;

        public const long VERIFIED_BLOCKS = 8;

        public const long LOCK_TIME_MAX = 31536000000; //micro senconds, 1 year

        public const long MAX_SIGNATURE = 10;

        public const long COINBASE_MATURITY = 100;

        public const long INPUT_AMOUNT_MAX = (long)4.5E+17;

        public const long OUTPUT_AMOUNT_MAX = (long)4.5E+17;

        public const long TRANSACTION_MIN_SIZE = 100;

        public const long TRANSACTION_MIN_FEE = 100;
    }
}
