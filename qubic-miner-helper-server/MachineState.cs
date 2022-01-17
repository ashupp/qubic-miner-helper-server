﻿using System;
using System.Collections.Generic;

namespace qubic_miner_helper_server
{
    public class MachineState
    {
        public string machineName;
        public float overallCurrentIterationsPerSecond;
        public float overallSessionErrorsFound;
        public int overallWorkerCount;
        public int overallThreadCount;
        public DateTime currentMachineDateTime;
        public DateTime lastErrorReductionByMachineDateTime;
        public string currentCommandLine;
        public string currentMinerVersion;
        public string currentHelperVersion;
        public string currentMinerPath;
        public List<WorkerState> currentWorkerStates;
    }
}