// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("WoGz3+PKS4DqbgVzO5tWIBgBfzaltr+bnLUdnzJAxH2V5/bEt49gNk61if1mjbbWKD7ylp2qfWkhKkk7ev8X9luHVYd6m501har3aJvnhoGpYpkOOkn2pDjAbP/FGow6RPKfu6NY+cjEYDby98TH3HStp/gVzDBnaiFrYb2ShitaOPY1E4K22Hn7LRAtzmw+f19ULkjgdUlKNhbAl3pqjSGb4oXB8Q3CUyQWVXPcSdWcLsY5X1D0+rS2jnazCmXEDHmfaH2kD16DC2cPjnzy0/d6YRY2ft91nGzLY6mW6C/cCIMJ50Al6sBvzYkr7hbTP7yyvY0/vLe/P7y8vXtsxuqEULKNP7yfjbC7tJc79TtKsLy8vLi9voEeMO5j0bw2tr++vL28");
        private static int[] order = new int[] { 12,7,10,11,10,9,11,10,11,13,13,11,13,13,14 };
        private static int key = 189;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
