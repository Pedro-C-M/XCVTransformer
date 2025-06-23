namespace XCVTransformer.Transformers
{
    public interface ITraductor : ITransformer
    {
        void ChangeOriginCode(string newCode);
        void ChangeEndCode(string newCode);
        bool SameFromTo();
    }
}
