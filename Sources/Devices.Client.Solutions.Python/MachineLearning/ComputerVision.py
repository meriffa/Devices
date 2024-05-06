#!/usr/bin/python3

import diffusers
import io
import matplotlib.pyplot
import numpy
import PIL
import random
import torch
import transformers
import transformers.image_transforms
import warnings


# Computer Vision models
class ComputerVision:

    # Initialization
    def __init__(self):
        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        print(f"Device: {self.device.upper()}")

    # Depth Estimation Auto (https://huggingface.co/LiheYoung/depth-anything-base-hf)
    def DepthEstimationAuto(self, image, showImages=False):
        processor = transformers.AutoImageProcessor.from_pretrained("LiheYoung/depth-anything-base-hf")
        model = transformers.AutoModelForDepthEstimation.from_pretrained("LiheYoung/depth-anything-base-hf")
        self.DepthEstimation(image, processor, model, showImages)

    # Depth Estimation DPT (https://huggingface.co/Intel/dpt-large)
    def DepthEstimationDPT(self, image, showImages=False):
        processor = transformers.DPTImageProcessor.from_pretrained("Intel/dpt-large")
        model = transformers.DPTForDepthEstimation.from_pretrained("Intel/dpt-large")
        self.DepthEstimation(image, processor, model, showImages)

    # Depth Estimation
    def DepthEstimation(self, image, processor, model, showImages=False):
        inputs = processor(images=image, return_tensors="pt")
        with torch.no_grad():
            output = model(**inputs)
        predictions = torch.nn.functional.interpolate(output.predicted_depth.unsqueeze(1), size=image.size[::-1], mode="bicubic", align_corners=False)
        print(predictions.shape)
        if showImages:
            pixels = predictions.squeeze().cpu().numpy()
            formatted = (pixels * 255 / numpy.max(pixels)).astype("uint8")
            depth = PIL.Image.fromarray(formatted)
            depth.show()

    #  Image Classification ViT (https://huggingface.co/google/vit-base-patch16-224)
    def ImageClassificationViT(self, image):
        MIN_ACCEPTABLE_SCORE = 0.1
        pipeline = transformers.pipeline("image-classification", model="google/vit-base-patch16-224")
        predictions = pipeline(image)
        for prediction in predictions:
            if prediction["score"] >= MIN_ACCEPTABLE_SCORE:
                print(f'Label = {prediction["label"]}, Score = {prediction["score"]}')

    #  Image Classification BEiT (https://huggingface.co/microsoft/beit-base-patch16-224-pt22k-ft22k)
    def ImageClassificationBEiT(self, image):
        processor = transformers.BeitImageProcessor.from_pretrained("microsoft/beit-base-patch16-224-pt22k-ft22k")
        model = transformers.BeitForImageClassification.from_pretrained("microsoft/beit-base-patch16-224-pt22k-ft22k")
        inputs = processor(images=image, return_tensors="pt")
        predictions = model(**inputs)
        print(f"Label = {model.config.id2label[predictions.logits.argmax(-1).item()]}")

    # Image Segmentation DETR (https://huggingface.co/facebook/detr-resnet-50-panoptic)
    def ImageSegmentationDETRMask(self, image, showImages=False):
        pipeline = transformers.pipeline(model="facebook/detr-resnet-50-panoptic")
        predictions = pipeline(image)
        for prediction in predictions:
            print(f'Label = {prediction["label"]}, Score = {prediction["score"]}')
            if showImages:
                mask = numpy.asarray(prediction["mask"]) / 255
                imageBuffer = numpy.asarray(image)
                maskChannel = numpy.zeros_like(imageBuffer)
                maskChannel[:, :, 0] = mask
                maskChannel[:, :, 1] = mask
                maskChannel[:, :, 2] = mask
                segmentedImage = imageBuffer * maskChannel
                PIL.Image.fromarray(segmentedImage.astype(numpy.uint8)).show()

    # Image Segmentation DETR (https://huggingface.co/facebook/detr-resnet-101)
    def ImageSegmentationDETRBox(self, image, showImages=False):
        pipeline = transformers.pipeline(model="facebook/detr-resnet-101")
        predictions = pipeline(image)
        for prediction in predictions:
            print(f'Label = {prediction["label"]}, Score = {prediction["score"]}')
        if showImages:
            self.GetObjectDetectionImage(image, predictions).show()

    # Image To Image (https://huggingface.co/caidas/swin2SR-classical-sr-x2-64)
    def ImageToImage(self, image, showImages=False):
        image = image.resize((64, 64))
        pipeline = transformers.pipeline("image-to-image", model="caidas/swin2SR-classical-sr-x2-64")
        output = pipeline(image)
        if showImages:
            image.show()
            output.show()

    # Image To Image (https://huggingface.co/timbrooks/instruct-pix2pix)
    def ImageToImageSD(self, image, prompt, showImages=False):
        image = PIL.ImageOps.exif_transpose(image)
        image = image.convert("RGB")
        pipeline = diffusers.StableDiffusionInstructPix2PixPipeline.from_pretrained("timbrooks/instruct-pix2pix", torch_dtype=torch.float16, safety_checker=None)
        pipeline.to(self.device)
        pipeline.scheduler = diffusers.EulerAncestralDiscreteScheduler.from_config(pipeline.scheduler.config)
        if self.device == "cpu":
            warnings.warn("Stable Diffusion model requires GPU acceleration.")
            return
        output = pipeline(prompt, image=image, num_inference_steps=10, image_guidance_scale=1).images
        if showImages:
            output[0].show()

    # Return Object Detection image
    def GetObjectDetectionImage(self, image, predictions):
        COLORS = ["#ff7f7f", "#ff7fbf", "#ff7fff", "#bf7fff", "#7f7fff", "#7fbfff", "#7fffff", "#7fffbf", "#7fff7f", "#bfff7f", "#ffff7f", "#ffbf7f"]
        matplotlib.pyplot.figure(figsize=(16, 10))
        matplotlib.pyplot.imshow(image)
        axes = matplotlib.pyplot.gca()
        for prediction in predictions:
            color = random.choice(COLORS)
            x, y = (prediction["box"]["xmin"], prediction["box"]["ymin"])
            w, h = prediction["box"]["xmax"] - prediction["box"]["xmin"], prediction["box"]["ymax"] - prediction["box"]["ymin"]
            axes.add_patch(matplotlib.pyplot.Rectangle((x, y), w, h, fill=False, color=color, linewidth=3))
            axes.text(x, y, f"{prediction['label'].title()}: {round(prediction['score']*100, 1)}%", fontfamily="DejaVu Sans", fontsize=15, color=color)
        matplotlib.pyplot.axis("off")
        figure = matplotlib.pyplot.gcf()
        buffer = io.BytesIO()
        figure.savefig(buffer, bbox_inches="tight")
        buffer.seek(0)
        return PIL.Image.open(buffer)

    # Object Detection (https://huggingface.co/facebook/detr-resnet-50)
    def ObjectDetection(self, image, showImages=False):
        pipeline = transformers.pipeline(model="facebook/detr-resnet-50")
        predictions = pipeline(image)
        for prediction in predictions:
            print(f'Label = {prediction["label"]}, Score = {prediction["score"]}')
        if showImages:
            self.GetObjectDetectionImage(image, predictions).show()

    # Zero-Shot Image Classification (https://huggingface.co/openai/clip-vit-large-patch14)
    def ZeroShotImageClassification(self, image):
        pipeline = transformers.pipeline(task="zero-shot-image-classification", model="openai/clip-vit-large-patch14")
        predictions = pipeline(image, candidate_labels=["black and white", "animals", "photorealist", "birds", "painting"])
        for prediction in predictions:
            print(f'Label = {prediction["label"]}, Score = {prediction["score"]}')

    # Zero-Shot Object Detection ()
    def ZeroShotObjectDetection(self, image, showImages=False):
        pipeline = transformers.pipeline(model="google/owlvit-base-patch32", task="zero-shot-object-detection")
        predictions = pipeline(image, candidate_labels=["cat", "couch"])
        for prediction in predictions:
            print(f'Label = {prediction["label"]}, Score = {prediction["score"]}')
        if showImages:
            self.GetObjectDetectionImage(image, predictions).show()
