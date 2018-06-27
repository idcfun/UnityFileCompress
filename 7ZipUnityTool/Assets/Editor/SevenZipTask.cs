using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SevenZipTask {

    private enum FileType
    {
        File = 0,
        Dir = 1,
        Default = 2
    }
    
    public static void StartCompress(string[] inputPaths, string outputPath)
    {
        SevenZipTask task = new SevenZipTask();
        task.Compress(inputPaths, outputPath);
    }

    private void Compress(string[] inputPaths, string outputPath)
    {
        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();

        string inputPath = inputPaths[0];
        int index = inputPath.LastIndexOf('.');
        outputPath = inputPath.Substring(0, index) + ".zip";
        FileStream output = new FileStream(outputPath, FileMode.Create);

        //Write the encoder properties
        coder.WriteCoderProperties(output);
        CompressAllPath(inputPaths, output, coder);
        output.Close();
    }


    /// <summary>
    /// 路径类型
    /// </summary>
    /// <returns></returns>
    private FileType GetPathType(string path__)
    {
        if (File.Exists(path__))
            return FileType.File;
        else if (Directory.Exists(path__))
            return FileType.Dir;
        else
            return FileType.Default;

    }

    private void CompressAllPath(string[] inputPaths, FileStream output, SevenZip.Compression.LZMA.Encoder coder)
    {
        int i = 0;
        FileStream input;
        int Len = inputPaths.Length;
        while (i < Len)
        {
            string path = inputPaths[i];

            if (GetPathType(path) == FileType.Dir)
            {
                CompressAllPath(Directory.GetFiles(path), output, coder);
                CompressAllPath(Directory.GetDirectories(path), output, coder);
                ++i;
                continue;
            }

            input = new FileStream(path, FileMode.Open);
            long inputStreamLen = input.Length;

            ///Write the file name 
            string filename = GetFileNameFromPath(path);

            byte[] nameBytes = UTF8Encoding.Default.GetBytes(filename);
            output.Write(BitConverter.GetBytes(nameBytes.Length), 0, 4);
            output.Write(nameBytes, 0, nameBytes.Length);

            // Write the file size.
            byte[] bytes = BitConverter.GetBytes(inputStreamLen);
            output.Write(bytes, 0, 8);

            // Encode the file.
            
            coder.Code(input, output, inputStreamLen, 0, null);
            output.Flush();
            input.Close();
            ++i;

        }
    }

    private string GetFileNameFromPath(string path_)
    {
        string[] pathSplit = path_.Split('/');
        return pathSplit[pathSplit.Length - 1];
    }

    private void Decompress(string inputPath)
    {
        SevenZip.Compression.LZMA.Decoder coder = new SevenZip.Compression.LZMA.Decoder();
        FileStream input = new FileStream(inputPath, FileMode.Open);

        input.Position = 0;
        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);
        coder.SetDecoderProperties(properties);

        while (input.Position < input.Length)
        {
            byte[] nameLenBytes = new byte[4];
            input.Read(nameLenBytes, 0, 4);
            int nameLen = BitConverter.ToInt32(nameLenBytes, 0);
            byte[] nameBytes = new byte[nameLen];
            input.Read(nameBytes, 0, nameLen);
            string name = UTF8Encoding.Default.GetString(nameBytes);

            byte[] fileLengthBytes = new byte[8];
            input.Read(fileLengthBytes, 0, 8);
            long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

            //FileStream output = new FileStream(DIR + "/" + name, FileMode.Create);
            //coder.Code(input, output, input.Length, fileLength, null);
            //output.Flush();
            //output.Close();
        }
        // Read in the decompress file size.


        // Decompress the file.

        //
        input.Close();
    }
}
