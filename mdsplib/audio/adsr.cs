using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace mdsplib.Audio
{
    public class ADSR
    {
        public enum StateEnum
        {
            env_idle,
            env_attack,
            env_decay,
            env_sustain,
            env_release
        };

        const double minvalue = 0.000000001;

        double output { get; set; }
        double attackCoef;
        double decayCoef;
        double releaseCoef;
        float targetRatioA;
        float targetRatioDR;
        float attackBase;
        float decayBase;
        float releaseBase;

        private double calc(double val)
        {
            return 0;  //(val <= 0) ? 0.0 : System.Math.Exp(-System.Math.Log((1.0 + targetRatioA) / targetRatioDR) / rate);
        }

        public StateEnum State { get; set; }

        ADSR(double attack, double deacy, double release, double sustain)
        {
        }

        public void Reset()
        {
            State = StateEnum.env_idle;
            output = 0.0;
        }

        void TargetRatioA(float targetRatio)
        {
            //if (targetRatio < minvalue)
            //    targetRatio = minvalue;  // -180 dB
            //targetRatioA = targetRatio;
            //attackCoef = calcCoef(attackRate, targetRatioA);
            //attackBase = (1.0 + targetRatioA) * (1.0 - attackCoef);
        }

        void TargetRatioDR(float targetRatio)
        {
            //if (targetRatio < minvalue)
            //    targetRatio = minvalue;  // -180 dB

            //targetRatioDR = targetRatio;
            //decayCoef = calcCoef(decayRate, targetRatioDR);
            //releaseCoef = calcCoef(releaseRate, targetRatioDR);
            //decayBase = (sustainLevel - targetRatioDR) * (1.0 - decayCoef);
            //releaseBase = -targetRatioDR * (1.0 - releaseCoef);
        }

        double process()
        {
            switch (State)
            {
                case StateEnum.env_idle:
                    break;

                case StateEnum.env_attack:
                    output = attackBase + output * attackCoef;
                    if (output >= 1.0) {
                        output = 1.0;
                        State = StateEnum.env_decay;
                    }
                    break;

                case StateEnum.env_decay:
                    //output = decayBase + output * decayCoef;
                    //if (output <= sustainLevel) {
                    //    output = sustainLevel;
                    //    State = StateEnum.env_sustain;
                    //}
                    break;

                case StateEnum.env_sustain:
                    break;

                case StateEnum.env_release:
                    output = releaseBase + output * releaseCoef;
                    if (output <= 0.0) {
                        output = 0.0;
                        State = StateEnum.env_idle;
                    }
                    break;
            }
            return output;
        }

        public void gate(bool g)
        {
            if (g)
                State = StateEnum.env_attack;
            else 
                if (State != StateEnum.env_idle)
                    State = StateEnum.env_release;
        }
    }
}