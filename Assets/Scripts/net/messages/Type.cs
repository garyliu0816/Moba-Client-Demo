// <auto-generated>
//   This file was generated by a tool; you should avoid making direct changes.
//   Consider using 'partial classes' to extend these types
//   Input: Type.proto
// </auto-generated>

#region Designer generated code
#pragma warning disable CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
namespace Messages.Type
{

    [global::ProtoBuf.ProtoContract()]
    public enum RequestType
    {
        [global::ProtoBuf.ProtoEnum(Name = @"LOGIN_REQUEST")]
        LoginRequest = 0,
        [global::ProtoBuf.ProtoEnum(Name = @"MATCH_REQUEST")]
        MatchRequest = 10,
        [global::ProtoBuf.ProtoEnum(Name = @"CANCEL_MATCH_REQUEST")]
        CancelMatchRequest = 11,
        [global::ProtoBuf.ProtoEnum(Name = @"BATTLE_READY_REQUEST")]
        BattleReadyRequest = 51,
        [global::ProtoBuf.ProtoEnum(Name = @"OPERATION_REQUEST")]
        OperationRequest = 53,
        [global::ProtoBuf.ProtoEnum(Name = @"DELTA_FRAMES_REQUEST")]
        DeltaFramesRequest = 55,
        [global::ProtoBuf.ProtoEnum(Name = @"GAME_OVER_REQUEST")]
        GameOverRequest = 57,
    }

    [global::ProtoBuf.ProtoContract()]
    public enum ResponseType
    {
        [global::ProtoBuf.ProtoEnum(Name = @"LOGIN_RESPONSE")]
        LoginResponse = 0,
        [global::ProtoBuf.ProtoEnum(Name = @"MATCH_RESPONSE")]
        MatchResponse = 10,
        [global::ProtoBuf.ProtoEnum(Name = @"CANCEL_MATCH_RESPONSE")]
        CancelMatchResponse = 11,
        [global::ProtoBuf.ProtoEnum(Name = @"ENTER_BATTLE_RESPONSE")]
        EnterBattleResponse = 50,
        [global::ProtoBuf.ProtoEnum(Name = @"BATTLE_START_RESPONSE")]
        BattleStartResponse = 51,
        [global::ProtoBuf.ProtoEnum(Name = @"OPERATIONS_RESPONSE")]
        OperationsResponse = 53,
        [global::ProtoBuf.ProtoEnum(Name = @"DELTA_FRAMES_RESPONSE")]
        DeltaFramesResponse = 55,
        [global::ProtoBuf.ProtoEnum(Name = @"GAME_OVER_RESPONSE")]
        GameOverResponse = 57,
    }

}

#pragma warning restore CS0612, CS0618, CS1591, CS3021, IDE0079, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
#endregion
