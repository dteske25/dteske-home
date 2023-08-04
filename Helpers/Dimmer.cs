using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeskeHomeAssistant.Helpers
{
    public class Dimmer
    {
        private int _max;
        private int _step;
        private int _possibleSteps;
        private int _currentStep;

        public Dimmer(int max, int step, int currentStep = 1)
        {
            _max = max;
            _step = step;
            _possibleSteps = max / step;
            _currentStep = currentStep;
        }

        public int Current => _max - _step * _currentStep;

        public int Next()
        {
            _currentStep = (_currentStep + 1) % _possibleSteps;
            return Current;
        }

        public void SetStep(int step)
        {
            if (step < 0)
            {
                throw new ArgumentOutOfRangeException("Step must be 0 or greater.");
            }
            if (step > _possibleSteps)
            {
                throw new ArgumentOutOfRangeException($"Step must be {_possibleSteps} or less.");
            }
            _currentStep = step;
        }
    }
}
