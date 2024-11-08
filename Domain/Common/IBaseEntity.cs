namespace Domain.Common;

public interface IBaseEntity<Tkey> where Tkey : IEquatable<Tkey>
{
    Tkey Id { get; set; }
}

public interface IBaseEntity: IBaseEntity<int>
{
}
