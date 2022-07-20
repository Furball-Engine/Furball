namespace Furball.Engine.Engine.Helpers; 

public class ArrayHelper {
    public static T[] FitElementsInANewArray <T>(T[] source, int targetLength) {
        int sourceLength        = source.Length;
        int baseSteps           = targetLength / sourceLength;
        int surplusElementCount = targetLength % sourceLength;

        T[] target = new T[targetLength];

        int offset = 0;

        for (int i = 0; i < sourceLength; i++) {
            T element = source[i];

            for (int j = 0; j < baseSteps; j++)
                target[offset + i * baseSteps + j] = element;

            if (surplusElementCount > 0) {
                target[offset++ + baseSteps * (i + 1)] = element;
                surplusElementCount--;
            }
        }

        return target;
    }
}