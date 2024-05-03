import easyocr
import io
import json
import matplotlib.patches
import matplotlib.pyplot
import numpy
import pandas
import PIL
import torch
import torchvision
import transformers


# Table Extraction model
class TableExtraction:

    # Image resize
    class MaxResize(object):

        # Initialization
        def __init__(self, max_size=800):
            self.maxSize = max_size

        # Execution
        def __call__(self, image):
            width, height = image.size
            currentMaxSize = max(width, height)
            scale = self.maxSize / currentMaxSize
            return image.resize((int(round(scale * width)), int(round(scale * height))))

    # Initialization
    def __init__(self):
        self.device = "cuda" if torch.cuda.is_available() else "cpu"
        print(f"Device: {self.device.upper()}")
        self.ocrReader = easyocr.Reader(["en"])

    # Convert bounding box coordinates
    def GetBoundingBoxCoordinates(self, boundingBox):
        xCenter, yCenter, width, height = boundingBox.unbind(-1)
        coordinates = [(xCenter - 0.5 * width), (yCenter - 0.5 * height), (xCenter + 0.5 * width), (yCenter + 0.5 * height)]
        return torch.stack(coordinates, dim=1)

    # Rescale bounding box
    def RescaleBoundingBox(self, boundingBox, size):
        width, height = size
        coordinates = self.GetBoundingBoxCoordinates(boundingBox)
        return coordinates * torch.tensor([width, height, width, height], dtype=torch.float32)

    # Detect table objects
    def DetectTableObjects(self, outputs, imgageSize, classLabels):
        max = outputs.logits.softmax(-1).max(-1)
        labels = list(max.indices.detach().cpu().numpy())[0]
        scores = list(max.values.detach().cpu().numpy())[0]
        boundingBoxes = outputs["pred_boxes"].detach().cpu()[0]
        boundingBoxes = [elem.tolist() for elem in self.RescaleBoundingBox(boundingBoxes, imgageSize)]
        objects = []
        for label, score, bbox in zip(labels, scores, boundingBoxes):
            classLabel = classLabels[int(label)]
            if not classLabel == "no object":
                objects.append({"label": classLabel, "score": float(score), "bbox": [float(elem) for elem in bbox]})
        return objects

    # Convert Matplotlib figure to PIL image
    def ConvertFigureToImage(self, figure):
        buffer = io.BytesIO()
        figure.savefig(buffer)
        buffer.seek(0)
        return PIL.Image.open(buffer)

    # Return detected tables as Matplotlib figure
    def GetDetectedTableFigures(self, image, detectedTables):
        matplotlib.pyplot.imshow(image, interpolation="lanczos")
        figure = matplotlib.pyplot.gcf()
        figure.set_size_inches(20, 20)
        axes = matplotlib.pyplot.gca()
        for detectedTable in detectedTables:
            boundingBox = detectedTable["bbox"]
            if detectedTable["label"] == "table":
                facecolor = (1, 0, 0.45)
                edgecolor = (1, 0, 0.45)
                alpha = 0.3
                linewidth = 2
                hatch = "//////"
            elif detectedTable["label"] == "table rotated":
                facecolor = (0.95, 0.6, 0.1)
                edgecolor = (0.95, 0.6, 0.1)
                alpha = 0.3
                linewidth = 2
                hatch = "//////"
            else:
                continue
            axes.add_patch(matplotlib.patches.Rectangle(boundingBox[:2], boundingBox[2] - boundingBox[0], boundingBox[3] - boundingBox[1], linewidth=linewidth, edgecolor="none", facecolor=facecolor, alpha=0.1))
            axes.add_patch(matplotlib.patches.Rectangle(boundingBox[:2], boundingBox[2] - boundingBox[0], boundingBox[3] - boundingBox[1], linewidth=linewidth, edgecolor=edgecolor, facecolor="none", linestyle="-", alpha=alpha))
            axes.add_patch(matplotlib.patches.Rectangle(boundingBox[:2], boundingBox[2] - boundingBox[0], boundingBox[3] - boundingBox[1], linewidth=0, edgecolor=edgecolor, facecolor="none", linestyle="-", hatch=hatch, alpha=0.2))
        matplotlib.pyplot.xticks([], [])
        matplotlib.pyplot.yticks([], [])
        legend = [matplotlib.patches.Patch(facecolor=(1, 0, 0.45), edgecolor=(1, 0, 0.45), label="Table", hatch="//////", alpha=0.3), matplotlib.patches.Patch(facecolor=(0.95, 0.6, 0.1), edgecolor=(0.95, 0.6, 0.1), label="Table (rotated)", hatch="//////", alpha=0.3)]
        matplotlib.pyplot.legend(handles=legend, bbox_to_anchor=(0.5, -0.02), loc="upper center", borderaxespad=0, fontsize=10, ncol=2)
        matplotlib.pyplot.gcf().set_size_inches(10, 10)
        matplotlib.pyplot.axis("off")
        return figure

    # Detect and crop table
    def DetectAndCropTable(self, image, showImages):
        # Prepare image
        detectionTransform = torchvision.transforms.Compose([TableExtraction.MaxResize(800), torchvision.transforms.ToTensor(), torchvision.transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])])
        pixelValues = detectionTransform(image).unsqueeze(0).to(self.device)
        # Load table recognition model
        model = transformers.AutoModelForObjectDetection.from_pretrained("microsoft/table-transformer-detection", revision="no_timm").to(self.device)
        # Forward pass
        with torch.no_grad():
            outputs = model(pixelValues)
        # Post-processing
        classLabels = model.config.id2label
        classLabels[len(model.config.id2label)] = "no object"
        detectedTables = self.DetectTableObjects(outputs, image.size, classLabels)
        # Visualize tables
        if showImages:
            self.ConvertFigureToImage(self.GetDetectedTableFigures(image, detectedTables)).show()
        # Crop first detected table
        return image.crop(detectedTables[0]["bbox"])

    # Detect table cells
    def DetectTableCells(self, image):
        # Prepare image
        structureTransform = torchvision.transforms.Compose([TableExtraction.MaxResize(1000), torchvision.transforms.ToTensor(), torchvision.transforms.Normalize([0.485, 0.456, 0.406], [0.229, 0.224, 0.225])])
        pixelValues = structureTransform(image).unsqueeze(0).to(self.device)
        # Load table structure recognition model
        model = transformers.AutoModelForObjectDetection.from_pretrained("microsoft/table-transformer-structure-recognition-v1.1-all").to(self.device)
        # Forward pass
        with torch.no_grad():
            outputs = model(pixelValues)
        # Post-processing
        classLabels = model.config.id2label
        classLabels[len(model.config.id2label)] = "no object"
        detectedCells = self.DetectTableObjects(outputs, image.size, classLabels)
        # visualize table cells
        draw = PIL.ImageDraw.Draw(image)
        for cell in detectedCells:
            draw.rectangle(cell["bbox"], outline="red")
        return image, detectedCells

    # Return cell bounding box
    def GetCellBoundingBox(self, row, column):
        cellBoundingBox = [column["bbox"][0], row["bbox"][1], column["bbox"][2], row["bbox"][3]]
        return cellBoundingBox

    # Return cell coordinates
    def GetCellCoordinates(self, table):
        rows = [entry for entry in table if entry["label"] == "table row"]
        columns = [entry for entry in table if entry["label"] == "table column"]
        rows.sort(key=lambda x: x["bbox"][1])
        columns.sort(key=lambda x: x["bbox"][0])
        cellCoordinates = []
        for row in rows:
            rowCells = []
            for column in columns:
                cell_bbox = self.GetCellBoundingBox(row, column)
                rowCells.append({"column": column["bbox"], "cell": cell_bbox})
            rowCells.sort(key=lambda x: x["column"][0])
            cellCoordinates.append({"row": row["bbox"], "cells": rowCells, "cell_count": len(rowCells)})
        cellCoordinates.sort(key=lambda x: x["row"][1])
        return cellCoordinates

    # Return cell data using OCR
    def GetCellDataUsingOCR(self, cellCoordinates, table):
        jsonData = dict()
        maxColumns = 0
        for index, row in enumerate(cellCoordinates):
            rowText = []
            for cell in row["cells"]:
                cellImage = numpy.array(table.crop(cell["cell"]))
                result = self.ocrReader.readtext(numpy.array(cellImage))
                if len(result) > 0:
                    text = " ".join([x[1] for x in result])
                    rowText.append(text)
            if len(rowText) > maxColumns:
                maxColumns = len(rowText)
            jsonData[str(index)] = rowText
        # Pad rows to make sure all rows have the same number of columns
        for index, rowData in jsonData.copy().items():
            if len(rowData) != maxColumns:
                rowData = rowData + ["" for _ in range(maxColumns - len(rowData))]
            jsonData[str(index)] = rowData
        return json.loads(json.dumps(jsonData))

    # Process image
    def ProcessImage(self, image, showImages):
        table = self.DetectAndCropTable(image, showImages)
        image, cells = self.DetectTableCells(table)
        cellCoordinates = self.GetCellCoordinates(cells)
        jsonData = self.GetCellDataUsingOCR(cellCoordinates, image)
        return jsonData, image

    # Detect table (https://huggingface.co/microsoft/table-transformer-detection)
    def DetectTable(self, image, showImages=False):
        if showImages:
            image.show()
        jsonData, outputImage = self.ProcessImage(image, showImages)
        if showImages:
            outputImage.show()
        dataFrame = pandas.DataFrame(jsonData)
        return jsonData, dataFrame, outputImage
