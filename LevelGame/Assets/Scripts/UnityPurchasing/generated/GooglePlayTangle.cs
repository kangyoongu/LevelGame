// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Smr87Gebp7nyCOs+sAk8vuZuWtulUTvAk5EKpfxdLAiQ+b3TD49Lwq+6RQE6wll1JEqIKAP5s0wBx+v8MFUqE5VRkdqyVSoNz2nowZ7qlDmU+RBQcjG8DymmzX8q86UbTA8mziX3Fpgibpa/K9a9CppyQTF18GIEjsMEzjV1e9BZryU2PnWtYkoLL95Z5mkMcyb6Hrgbpkltal3LkE1iUoHJc5Q8dgrqNdyuev3a3DFcE9AQqKZlFjczE4/VNqCMWky9KR/tUihg4+3i0mDj6OBg4+PiX3FJ+SnIPtJg48DS7+TryGSqZBXv4+Pj5+LhNGZ9SzToFVeRnPh4z8ISYZim9RKPdURVaTEY03b8E1qLwU/EPO4tHoZRDqY9xq32neDh4+Lj");
        private static int[] order = new int[] { 7,4,2,6,9,13,13,10,11,11,11,11,13,13,14 };
        private static int key = 226;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
