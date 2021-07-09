﻿namespace Utils.Document
{
    public interface IDocumentData
    {
        string Code { get; }
    }

    public class DocumentData : IDocumentData
    {
        public string Code { get; set; }
        public string Kor { get; set; }
        public string Eng { get; set; }
    }

    public interface IDocument
    {
        void Init(string path);
    }
}
