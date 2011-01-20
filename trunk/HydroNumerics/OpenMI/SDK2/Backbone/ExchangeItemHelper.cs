#region Copyright
/*
* Copyright (c) HydroInform ApS & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the HydroInform ApS & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "HydroInform ApS & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "HydroInform ApS & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
﻿using System;
using System.Collections.Generic;
using OpenMI.Standard2.TimeSpace;

namespace HydroNumerics.OpenMI.Sdk.Backbone
{
    public class ExchangeItemHelper
    {
        public static bool ConsumerCanBeAdded(IList<ITimeSpaceInput> existingConsumers, ITimeSpaceInput consumer)
        {
            bool canBeAdded = true;
            if (existingConsumers.Count > 0)
            {
                // TODO (JG): make instead check on exchangeItem.ElementSet instead of an arbitray elementSet
                // check if #elements are consistent
                if (consumer.ElementSet() != null &&
                    existingConsumers[0].ElementSet() != null &&
                    consumer.ElementSet().ElementCount != existingConsumers[0].ElementSet().ElementCount
                    )
                {
                    canBeAdded = false;
                }
                // TODO (JG): Is this a requirement?
                if (consumer.TimeSet != null &&
                    existingConsumers[0].TimeSet != null &&
                    consumer.TimeSet.HasDurations != existingConsumers[0].TimeSet.HasDurations
                    )
                {
                    canBeAdded = false;
                }
                // check if #time stamps are consistent
            }
            return canBeAdded;
        }

        public static bool OutputAndInputValuesFit(ITimeSpaceExchangeItem sourceItem, ITimeSpaceExchangeItem targetItem)
        {
            bool timeFits = OutputAndInputTimeSetsFit(sourceItem, targetItem);
            bool elementSetFits = true;
            if (timeFits)
            {
                elementSetFits = OutputAndInputElementSetsFit(sourceItem, targetItem);
            }            
            return timeFits && elementSetFits;
        }

        public static bool OutputAndInputTimeSetsFit(ITimeSpaceExchangeItem sourceItem, ITimeSpaceExchangeItem targetItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException("sourceItem");
            }

            if (targetItem == null)
            {
                throw new ArgumentNullException("targetItem");
            }

            bool timeFits = true;
            ITimeSet sourceTimeSet = sourceItem.TimeSet;
            ITimeSet targetTimeSet = targetItem.TimeSet;
            if (sourceTimeSet == null)
            {
                if (targetTimeSet != null)
                {
                    // NOTE: Source has no timeset specification, source has.
                    // Source fits target if target requires only one time step.
                    timeFits = targetTimeSet.Times.Count == 1;
                }
            }
            else
            {
                if (targetTimeSet == null)
                {
                    // NOTE: Target has no timeset specification, source has.
                    // Source fits target if source has values for only one time step available.
                    timeFits = sourceTimeSet.Times.Count == 1;
                }
                else
                {
                    /*
                     * SH/AM: TODO I Think this code is wrong, IOutput and IAdaptedOutput should be treated the same
                     * (SH: reactivated (if (sourceItem is IAdaptedOutput) code
                     * to make things work for time extrapolators again.
                     */
                    // Both source and target have time set specification
                    if (sourceItem is ITimeSpaceAdaptedOutput)
                    {
                        // NOTE: Source is an adaptedOutput that has a time set.
                        // Most probably a timeinterpolator, but:
                        // TODO: Check how we can find out that it is a time interpolator.
                        // For now: check if the target's last required time is included in the source's time horizon
                        if (sourceTimeSet.TimeHorizon == null)
                        {
                            throw new Exception("Error when checking if the times of AdaptedOutput \"" + sourceItem.Id +
                                                " fits the times required by inputItem \"" + targetItem.Id +
                                                "\": no time horizon available in the adaptedOutput");
                        }
                        ITime lastRequiredTime = targetTimeSet.Times[targetTimeSet.Times.Count - 1];
                        double lastRequiredTimeAsMJD = lastRequiredTime.StampAsModifiedJulianDay +
                                                       lastRequiredTime.DurationInDays;
                        double endOfSourceTimeHorizon = sourceTimeSet.TimeHorizon.StampAsModifiedJulianDay +
                                                        sourceTimeSet.TimeHorizon.DurationInDays;
                        timeFits = lastRequiredTimeAsMJD <= (endOfSourceTimeHorizon + Time.EpsilonForTimeCompare);
                    }
                    else
                    {
                        timeFits = false;
                        // regular (output) exchange item, check if all times fit
                        IList<ITime> sourceTimes = sourceTimeSet.Times;
                        IList<ITime> requiredTimes = targetTimeSet.Times;
                        if (sourceTimes.Count == requiredTimes.Count)
                        {
                            timeFits = true;
                            for (int timeIndex = 0; timeIndex < requiredTimes.Count; timeIndex++)
                            {
                                if ( (requiredTimes[timeIndex].DurationInDays > 0 && !(sourceTimes[timeIndex].DurationInDays>0)) ||
                                     (sourceTimes[timeIndex].DurationInDays > 0 && !(requiredTimes[timeIndex].DurationInDays>0)) )
                                {
                                    throw new Exception("Incompatible times (stamp versus span) between outputItem \"" + sourceItem.Id +
                                                        " and inputItem \"" + targetItem.Id + "\"");
                                }
                                if (requiredTimes[timeIndex].Equals(sourceTimes[timeIndex])) continue;
                                timeFits = false;
                                break;
                            }
                        }
                    }
                }
            }
            return timeFits;
        }

        public static bool OutputAndInputElementSetsFit(ITimeSpaceExchangeItem sourceItem, ITimeSpaceExchangeItem targetItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException("sourceItem");
            }

            if (targetItem == null)
            {
                throw new ArgumentNullException("targetItem");
            }

            bool elementSetFits = true;
            IElementSet sourceElementSet = sourceItem.ElementSet();
            IElementSet targetElementSet = targetItem.ElementSet();
            if (sourceElementSet == null)
            {
                if (targetElementSet != null)
                {
                    // NOTE: Source has no elementset specification, source has.
                    // Source fits target if target requires only one element.
                    elementSetFits = targetElementSet.ElementCount == 1;
                }
            }
            else
            {
                if (targetElementSet == null)
                {
                    // NOTE: Target has no elementset specification, source has.
                    // Source fits target if source has values for only one element available.
                    elementSetFits = sourceElementSet.ElementCount == 1;
                }
                else
                {
                    // Both source and target have an element set specification
                    // If the source is a regular exchange item, the #elements will fit
                    // (has been checked configuration time)

                    // If it is a spatial extension, we need to check if valeus on the newly required
                    // element set can be delivered
                    if (sourceItem is ITimeSpaceAdaptedOutput)
                    {
                        // TODO: Check how we can find out that it is a spatial adaptedOutput.
                        // TODO: If it is, how do we check whether the values on the target element set can be delivered
                    }
                }
            }
            return elementSetFits;
        }

        public static void CheckValueSizes(ITimeSpaceExchangeItem exchangeItem, ITimeSpaceValueSet valueSet)
        {
            int timesCount = 1;
            if (exchangeItem.TimeSet != null)
            {
                if (exchangeItem.TimeSet.Times != null)
                {
                    timesCount = exchangeItem.TimeSet.Times.Count;
                }
                else
                {
                    timesCount = 0;
                }
            }

            if (ValueSet.GetTimesCount(valueSet) != timesCount)
            {
                throw new Exception("ExchangeItem \"" + exchangeItem.Caption +
                    "\": Wrong #times in valueSet (" + ValueSet.GetTimesCount(valueSet) + "), expected #times (" + timesCount + ")");
            }

            int elementCount = 1;
            if (exchangeItem.ElementSet() != null)
            {
                elementCount = exchangeItem.ElementSet().ElementCount;
            }
            if (ValueSet.GetElementCount(valueSet) != elementCount)
            {
                throw new Exception("ExchangeItem \"" + exchangeItem.Caption +
                    "\": Wrong #times in valueSet (" + ValueSet.GetElementCount(valueSet) + "), expected #times (" + elementCount + ")");
            }

        }

        public static ITime DetermineLastQueryTime(IList<ITimeSpaceInput> consumers)
        {
            double lastTimeAsMJD = double.NegativeInfinity;
            foreach (ITimeSpaceInput inputItem in consumers)
            {
                if (inputItem.TimeSet != null && inputItem.TimeSet.Times != null)
                {
                    foreach (ITime time in inputItem.TimeSet.Times)
                    {
                        lastTimeAsMJD = Math.Max(lastTimeAsMJD, time.StampAsModifiedJulianDay + time.DurationInDays);
                    }
                }
            }
            if (lastTimeAsMJD == double.NegativeInfinity)
            {
                // no times found
                return null;
            }
            return new Time(lastTimeAsMJD);
        }
    }
}
