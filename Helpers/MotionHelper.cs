using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetDaemon.HassModel.Entities;

namespace TeskeHomeAssistant.Helpers
{
    public class MotionBuilder
    {
        private BinarySensorEntity _motionSensor;
        private InputBooleanEntity? _motionAllowed;
        private SwitchEntity? _motionAllowedSwitch;
        private Action<StateChange<BinarySensorEntity, EntityState<BinarySensorAttributes>>>? _onAction;
        private Action<StateChange<BinarySensorEntity, EntityState<BinarySensorAttributes>>>? _offAction;
        private TimeSpan? _motionTimeout;
        private IScheduler _scheduler;
        private ILogger _logger;

        public MotionBuilder(BinarySensorEntity motionSensor, IScheduler scheduler, ILogger logger)
        {
            _motionSensor = motionSensor;
            _scheduler = scheduler;
            _logger = logger;
        }

        public MotionBuilder WithMotionAllowed(InputBooleanEntity motionAllowed)
        {
            _motionAllowed = motionAllowed;
            return this;
        }

        public MotionBuilder WithMotionAllowed(SwitchEntity motionAllowed)
        {
            _motionAllowedSwitch = motionAllowed;
            return this;
        }

        public MotionBuilder WithOnAction(Action<StateChange<BinarySensorEntity, EntityState<BinarySensorAttributes>>> onAction)
        {
            _onAction = onAction;
            return this;
        }

        public MotionBuilder WithOffAction(Action<StateChange<BinarySensorEntity, EntityState<BinarySensorAttributes>>> offAction, TimeSpan? timeSpan = null)
        {
            _offAction = offAction;
            _motionTimeout = timeSpan;
            return this;
        }


        public void Build()
        {
            _motionSensor.StateChanges().Where(obv => obv.New?.State == "on").Subscribe(change =>
            {
                _logger.LogDebug("Detected motion {@MotionSensor}", _motionSensor);
                if (_motionAllowed?.State == "off")
                {
                    _logger.LogDebug("Motion not allowed {@MotionAllowed}", _motionAllowed);
                    return;
                }
                if (_motionAllowedSwitch?.State == "off")
                {
                    _logger.LogDebug("Motion not allowed {@MotionAllowed}", _motionAllowedSwitch);
                    return;
                }
                _onAction?.Invoke(change);
                _logger.LogInformation("On action called");
            });


            _motionSensor.StateChanges().WhenStateIsFor(obv => obv?.State == "off", _motionTimeout ?? TimeSpan.FromMinutes(15), _scheduler).Subscribe(change =>
            {
                _logger.LogDebug("Detected motion {@MotionSensor}", _motionSensor);
                if (_motionAllowed?.State == "off")
                {
                    _logger.LogDebug("Motion not allowed {@MotionAllowed}", _motionAllowed);
                    return;
                }
                if (_motionAllowedSwitch?.State == "off")
                {
                    _logger.LogDebug("Motion not allowed {@MotionAllowed}", _motionAllowedSwitch);
                    return;
                }
                _offAction?.Invoke(change);
                _logger.LogInformation("Off action called");
            });
        }
    }
}
