// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("LkOq6siLBrWTHHfFkEkfofa1nHQ1z/7v04uiacxGqeAxe/V+hlSXpDtzyS6GzLBQj2YUwEdgZovmqWqqn02sIpjULAWRbAewIMj7i89K2L5o2ll6aFVeUXLeEN6vVVlZWV1YWx/rgXopK7AfRueWsipDB2m1NfF42llXWGjaWVJa2llZWOXL80OTcoTw0EZW3SEdA0iyUYQKs4YEXNTgYYrvkKkv6ytgCO+Qt3XTUnskUC6DNHm+dI/PwWrjFZ+MhM8X2PCxlWQVAP+7gHjjz57wMpK5Qwn2u31RRuNc07bJnECkAqEc89fQ53Eq99jojtzH8Y5Sr+0rJkLCdXio2yIcT6gSHN+sjYmpNW+MGjbg9geTpVfokjzrtByHfBdMJ1pbWVhZ");
        private static int[] order = new int[] { 9,12,11,13,9,9,12,7,12,9,11,11,13,13,14 };
        private static int key = 88;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
