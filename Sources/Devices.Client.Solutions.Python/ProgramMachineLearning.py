#!/usr/bin/python3

import argparse
import MachineLearning.Audio
import MachineLearning.ComputerVision
import MachineLearning.Diffusion
import MachineLearning.NaturalLanguageProcessing
import MachineLearning.TableExtraction
import PIL
import requests


# Return image
def GetImage(url):
    return PIL.Image.open(requests.get(url, stream=True).raw)


# Audio tasks
def AudioTasks(modelIndex):
    audio = MachineLearning.Audio.Audio()
    match modelIndex:
        case 1:
            audio.AutomaticSpeechRecognition()


# Computer Vision tasks
def ComputerVisionTasks(modelIndex):
    computerVision = MachineLearning.ComputerVision.ComputerVision()
    match modelIndex:
        case 1:
            computerVision.DepthEstimationAuto(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"), showImages=True)
        case 2:
            computerVision.DepthEstimationDPT(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"), showImages=True)
        case 3:
            computerVision.ImageClassificationViT(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"))
        case 4:
            computerVision.ImageClassificationBEiT(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"))
        case 5:
            computerVision.ImageSegmentationDETRMask(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"), showImages=True)
        case 6:
            computerVision.ImageSegmentationDETRBox(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"), showImages=True)
        case 7:
            computerVision.ImageToImage(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"), showImages=True)
        case 8:
            computerVision.ImageToImageSD(GetImage("https://raw.githubusercontent.com/timothybrooks/instruct-pix2pix/main/imgs/example.jpg"), "Turn him into cyborg", showImages=True)
        case 9:
            computerVision.ObjectDetection(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"), showImages=True)
        case 10:
            computerVision.ZeroShotImageClassification(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"))
        case 11:
            computerVision.ZeroShotObjectDetection(GetImage("http://images.cocodataset.org/val2017/000000039769.jpg"), showImages=True)


# Diffusion tasks
def DiffusionTasks(modelIndex):
    diffusion = MachineLearning.Diffusion.Diffusion()
    match modelIndex:
        case 1:
            diffusion.StableDiffusion("An image of a rabbit in Monet style", modelName="CompVis/stable-diffusion-v1-4")
        case 2:
            diffusion.StableDiffusion("An image of a rabbit in Monet style", modelName="runwayml/stable-diffusion-v1-5")
        case 3:
            diffusion.StableDiffusion("An image of a rabbit in Monet style", modelName="stabilityai/stable-diffusion-xl-base-1.0")


# Natural Language Processing tasks
def NaturalLanguageProcessingTasks(modelIndex):
    nlp = MachineLearning.NaturalLanguageProcessing.NaturalLanguageProcessing()
    match modelIndex:
        case 1:
            nlp.SentimentAnalysis()
        case 2:
            nlp.ZeroShotClassification()
        case 3:
            nlp.TextGeneration("openai-community/gpt2")
        case 4:
            nlp.TextGeneration("distilbert/distilgpt2")
        case 5:
            nlp.TextGenerationLlama("meta-llama/Meta-Llama-3-8B")
        case 6:
            nlp.MaskFilling()
        case 7:
            nlp.NamedEntityRecognition("dbmdz/bert-large-cased-finetuned-conll03-english")
        case 8:
            nlp.NamedEntityRecognition("dslim/bert-base-NER")
        case 9:
            nlp.QuestionAnswering()
        case 10:
            nlp.Summarization("sshleifer/distilbart-cnn-12-6")
        case 11:
            nlp.Summarization("facebook/bart-large-cnn")
        case 12:
            nlp.Translation("t5-small", "My name is Peter and I live in Wales.", "English", "French")
        case 13:
            nlp.Translation("t5-large", "My name is Peter and I live in Wales.", "English", "German")


# Table Extraction tasks
def TableExtractionTasks(modelIndex):
    table = MachineLearning.TableExtraction.TableExtraction()
    match modelIndex:
        case 1:
            jsonData, dataFrame, _ = table.DetectTable(GetImage("https://huggingface.co/spaces/nielsr/tatr-demo/resolve/main/image.png?download=true"), showImages=True)
            print(jsonData)
            print(dataFrame)


# Application startup
def Main():
    parser = argparse.ArgumentParser(prog="Devices.Client.Solutions.Python", add_help=False)
    parser.add_argument("-t", "--task", help="Execute tasks.", required=True, choices=["Audio", "ComputerVision", "Diffusion", "NaturalLanguageProcessing", "TableExtraction"])
    parser.add_argument("-m", "--model", help="Model index.", required=True, type=int)
    parser = parser.parse_args()
    if parser.task == "Audio":
        AudioTasks(parser.model)
    elif parser.task == "ComputerVision":
        ComputerVisionTasks(parser.model)
    elif parser.task == "Diffusion":
        DiffusionTasks(parser.model)
    elif parser.task == "NaturalLanguageProcessing":
        NaturalLanguageProcessingTasks(parser.model)
    elif parser.task == "TableExtraction":
        TableExtractionTasks(parser.model)


if __name__ == "__main__":
    Main()
