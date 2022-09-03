namespace Furball.Engine.Engine.Helpers; 

public static class ArrayHelper {
    public static pT[] FitElementsInANewArray <pT>(pT[] source, int targetLength) {
        int sourceLength        = source.Length;
        int baseSteps           = targetLength / sourceLength;
        int surplusElementCount = targetLength % sourceLength;

        pT[] target = new pT[targetLength];

        int offset = 0;

        for (int i = 0; i < sourceLength; i++) {
            pT element = source[i];

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