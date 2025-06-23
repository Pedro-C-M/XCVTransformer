namespace XCVTransformer.Transformers.Codificators
{
    public interface ICodificator : ITransformer
    {
        void ChangeCodificatorMode(bool newMode);
    }
}
