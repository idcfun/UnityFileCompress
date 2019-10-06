using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ZipTask {

    
    public static void StartCompress(string url, string[] inputPaths, string outputPath)
    {
        ZipTask task = new ZipTask();
        task.Compress(url, inputPaths, outputPath);
    }

    public static void StartDecompress(string decompressfile)
    {
        ZipTask task = new ZipTask();
        task.Decompress(decompressfile);
    }

    private void Compress(string url, string[] inputPaths, string outputPath)
    {
        SevenZip.Compression.LZMA.Encoder coder = new SevenZip.Compression.LZMA.Encoder();
        FileStream output = new FileStream(outputPath, FileMode.Create);
        //Write the encoder properties
        coder.WriteCoderProperties(output);
        FileStream input;
        int fileCount = inputPaths.Length;
        byte[] bytes = BitConverter.GetBytes(fileCount);
        output.Write(bytes, 0, 4);
        for (int i = 0; i < fileCount; i++)
        {
            string path = inputPaths[i];
            input = new FileStream(path, FileMode.Open);
            long inputStreamLen = input.Length;

            ///Write the file name 
            string filename = path.Substring(url.Length+1, path.Length-url.Length-1);
            Debug.Log(filename);

            byte[] nameBytes = Encoding.Default.GetBytes(filename);
            output.Write(BitConverter.GetBytes(nameBytes.Length), 0, 4);
            output.Write(nameBytes, 0, nameBytes.Length);

            // Write the file size.
            bytes = BitConverter.GetBytes(inputStreamLen);
            output.Write(bytes, 0, 8);

            // Encode the file.

            coder.Code(input, output, inputStreamLen, -1, null);
            output.Flush();
            input.Close();
        }

        output.Close();
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
        byte[] bytes = new byte[4];
        input.Read(bytes, 0, 4);
        int fileCount = BitConverter.ToInt32(bytes, 0);

        for(int i = 0; i < fileCount; i++)
        {
            bytes = new byte[4];
            input.Read(bytes, 0, 4);
            int nameLength = BitConverter.ToInt32(bytes, 0);
            bytes = new byte[nameLength];
            input.Read(bytes, 0, nameLength);
            string name = Encoding.Default.GetString(bytes);
            string dir = Application.streamingAssetsPath + "/"+Path.GetDirectoryName(name);

            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Debug.Log(dir);
            }

            bytes = new byte[8];
            input.Read(bytes, 0, 8);
            long fileSize = BitConverter.ToInt64(bytes, 0);
            FileStream output = new FileStream(Application.streamingAssetsPath + "/" + name, FileMode.OpenOrCreate);
            coder.Code(input, output, input.Length, fileSize, null);
            output.Flush();
            output.Close();
        }

        input.Close();
    }
}
