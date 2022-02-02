using System;

namespace Math.Api {
    public class SimpleMath {
        public int add (int a, int b) {
            return a + b;
        }

        public int substract (int a, int b) {
            return a - b;
        }

        public decimal divide (int a, int b) {
            if (b == 0) {
                throw new DivideByZeroException ();
            } else {

                return a / b;
            }
        }

    }

}