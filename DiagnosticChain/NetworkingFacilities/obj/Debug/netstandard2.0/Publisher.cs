// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/publisher.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from Protos/publisher.proto</summary>
public static partial class PublisherReflection {

  #region Descriptor
  /// <summary>File descriptor for Protos/publisher.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static PublisherReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChZQcm90b3MvcHVibGlzaGVyLnByb3RvIksKCkFja01lc3NhZ2USIgoGc3Rh",
          "dHVzGAEgASgOMhIuQWNrTWVzc2FnZS5TdGF0dXMiGQoGU3RhdHVzEgYKAk9L",
          "EAASBwoDTk9LEAEiGwoMQ2hhaW5NZXNzYWdlEgsKA3htbBgBIAEoCSIkCgxE",
          "ZWx0YVJlcXVlc3QSFAoMY3VycmVudEluZGV4GAEgASgDIg0KC1BpbmdSZXF1",
          "ZXN0IjAKFFNlcnZlckFkZHJlc3NNZXNzYWdlEgoKAmlwGAEgASgJEgwKBHBv",
          "cnQYAiABKAUyiwIKD1B1Ymxpc2hlclNlcnZlchIjCgRQaW5nEgwuUGluZ1Jl",
          "cXVlc3QaCy5BY2tNZXNzYWdlIgASLAoMUmVjZWl2ZUNoYWluEg0uQ2hhaW5N",
          "ZXNzYWdlGgsuQWNrTWVzc2FnZSIAEjQKDFJlZ2lzdGVyTm9kZRIVLlNlcnZl",
          "ckFkZHJlc3NNZXNzYWdlGgsuQWNrTWVzc2FnZSIAEjMKEVJlcXVlc3REZWx0",
          "YUNoYWluEg0uRGVsdGFSZXF1ZXN0Gg0uQ2hhaW5NZXNzYWdlIgASOgoQUmVx",
          "dWVzdEZ1bGxDaGFpbhIVLlNlcnZlckFkZHJlc3NNZXNzYWdlGg0uQ2hhaW5N",
          "ZXNzYWdlIgBCMAoWZGlhZ25vc3RpY2NoYWluLnByb3Rvc0IOUHVibGlzaGVy",
          "UHJvdG9QAaICA1B1UGIGcHJvdG8z"));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::AckMessage), global::AckMessage.Parser, new[]{ "Status" }, null, new[]{ typeof(global::AckMessage.Types.Status) }, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::ChainMessage), global::ChainMessage.Parser, new[]{ "Xml" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::DeltaRequest), global::DeltaRequest.Parser, new[]{ "CurrentIndex" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::PingRequest), global::PingRequest.Parser, null, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::ServerAddressMessage), global::ServerAddressMessage.Parser, new[]{ "Ip", "Port" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class AckMessage : pb::IMessage<AckMessage> {
  private static readonly pb::MessageParser<AckMessage> _parser = new pb::MessageParser<AckMessage>(() => new AckMessage());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<AckMessage> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PublisherReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public AckMessage() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public AckMessage(AckMessage other) : this() {
    status_ = other.status_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public AckMessage Clone() {
    return new AckMessage(this);
  }

  /// <summary>Field number for the "status" field.</summary>
  public const int StatusFieldNumber = 1;
  private global::AckMessage.Types.Status status_ = global::AckMessage.Types.Status.Ok;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::AckMessage.Types.Status Status {
    get { return status_; }
    set {
      status_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as AckMessage);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(AckMessage other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Status != other.Status) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Status != global::AckMessage.Types.Status.Ok) hash ^= Status.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Status != global::AckMessage.Types.Status.Ok) {
      output.WriteRawTag(8);
      output.WriteEnum((int) Status);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Status != global::AckMessage.Types.Status.Ok) {
      size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Status);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(AckMessage other) {
    if (other == null) {
      return;
    }
    if (other.Status != global::AckMessage.Types.Status.Ok) {
      Status = other.Status;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          Status = (global::AckMessage.Types.Status) input.ReadEnum();
          break;
        }
      }
    }
  }

  #region Nested types
  /// <summary>Container for nested types declared in the AckMessage message type.</summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static partial class Types {
    public enum Status {
      [pbr::OriginalName("OK")] Ok = 0,
      [pbr::OriginalName("NOK")] Nok = 1,
    }

  }
  #endregion

}

public sealed partial class ChainMessage : pb::IMessage<ChainMessage> {
  private static readonly pb::MessageParser<ChainMessage> _parser = new pb::MessageParser<ChainMessage>(() => new ChainMessage());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ChainMessage> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PublisherReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChainMessage() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChainMessage(ChainMessage other) : this() {
    xml_ = other.xml_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ChainMessage Clone() {
    return new ChainMessage(this);
  }

  /// <summary>Field number for the "xml" field.</summary>
  public const int XmlFieldNumber = 1;
  private string xml_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Xml {
    get { return xml_; }
    set {
      xml_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ChainMessage);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ChainMessage other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Xml != other.Xml) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Xml.Length != 0) hash ^= Xml.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Xml.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Xml);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Xml.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Xml);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ChainMessage other) {
    if (other == null) {
      return;
    }
    if (other.Xml.Length != 0) {
      Xml = other.Xml;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          Xml = input.ReadString();
          break;
        }
      }
    }
  }

}

public sealed partial class DeltaRequest : pb::IMessage<DeltaRequest> {
  private static readonly pb::MessageParser<DeltaRequest> _parser = new pb::MessageParser<DeltaRequest>(() => new DeltaRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<DeltaRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PublisherReflection.Descriptor.MessageTypes[2]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public DeltaRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public DeltaRequest(DeltaRequest other) : this() {
    currentIndex_ = other.currentIndex_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public DeltaRequest Clone() {
    return new DeltaRequest(this);
  }

  /// <summary>Field number for the "currentIndex" field.</summary>
  public const int CurrentIndexFieldNumber = 1;
  private long currentIndex_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public long CurrentIndex {
    get { return currentIndex_; }
    set {
      currentIndex_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as DeltaRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(DeltaRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (CurrentIndex != other.CurrentIndex) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (CurrentIndex != 0L) hash ^= CurrentIndex.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (CurrentIndex != 0L) {
      output.WriteRawTag(8);
      output.WriteInt64(CurrentIndex);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (CurrentIndex != 0L) {
      size += 1 + pb::CodedOutputStream.ComputeInt64Size(CurrentIndex);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(DeltaRequest other) {
    if (other == null) {
      return;
    }
    if (other.CurrentIndex != 0L) {
      CurrentIndex = other.CurrentIndex;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          CurrentIndex = input.ReadInt64();
          break;
        }
      }
    }
  }

}

public sealed partial class PingRequest : pb::IMessage<PingRequest> {
  private static readonly pb::MessageParser<PingRequest> _parser = new pb::MessageParser<PingRequest>(() => new PingRequest());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<PingRequest> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PublisherReflection.Descriptor.MessageTypes[3]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PingRequest() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PingRequest(PingRequest other) : this() {
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public PingRequest Clone() {
    return new PingRequest(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as PingRequest);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(PingRequest other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(PingRequest other) {
    if (other == null) {
      return;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
      }
    }
  }

}

public sealed partial class ServerAddressMessage : pb::IMessage<ServerAddressMessage> {
  private static readonly pb::MessageParser<ServerAddressMessage> _parser = new pb::MessageParser<ServerAddressMessage>(() => new ServerAddressMessage());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<ServerAddressMessage> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::PublisherReflection.Descriptor.MessageTypes[4]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ServerAddressMessage() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ServerAddressMessage(ServerAddressMessage other) : this() {
    ip_ = other.ip_;
    port_ = other.port_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public ServerAddressMessage Clone() {
    return new ServerAddressMessage(this);
  }

  /// <summary>Field number for the "ip" field.</summary>
  public const int IpFieldNumber = 1;
  private string ip_ = "";
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public string Ip {
    get { return ip_; }
    set {
      ip_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
    }
  }

  /// <summary>Field number for the "port" field.</summary>
  public const int PortFieldNumber = 2;
  private int port_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Port {
    get { return port_; }
    set {
      port_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as ServerAddressMessage);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(ServerAddressMessage other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (Ip != other.Ip) return false;
    if (Port != other.Port) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (Ip.Length != 0) hash ^= Ip.GetHashCode();
    if (Port != 0) hash ^= Port.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (Ip.Length != 0) {
      output.WriteRawTag(10);
      output.WriteString(Ip);
    }
    if (Port != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(Port);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (Ip.Length != 0) {
      size += 1 + pb::CodedOutputStream.ComputeStringSize(Ip);
    }
    if (Port != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Port);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(ServerAddressMessage other) {
    if (other == null) {
      return;
    }
    if (other.Ip.Length != 0) {
      Ip = other.Ip;
    }
    if (other.Port != 0) {
      Port = other.Port;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          Ip = input.ReadString();
          break;
        }
        case 16: {
          Port = input.ReadInt32();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
