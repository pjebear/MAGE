﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class ActionResult
{
    public EncounterCharacter Initiator;
    public ActionId ActionId;
    public ActionInfo ActionInfo;
    public InteractionResult InitiatorResult;
    public Dictionary<EncounterCharacter, InteractionResult> TargetResults;

    public ActionResult(EncounterCharacter initiator, ActionId actionId, ActionInfo actionInfo, InteractionResult initiatorResult, Dictionary<EncounterCharacter, InteractionResult> targetResults)
    {
        Initiator = initiator;
        ActionId = actionId;
        ActionInfo = actionInfo;
        InitiatorResult = initiatorResult;
        TargetResults = targetResults;
    }
}
