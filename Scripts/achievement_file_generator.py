"""
Converts an achievement CSV to Terraria localization and RetroAchievements mod integration files

CSV format: Name, Title, Description, Points
"""

import argparse
import csv
import json


class AchievementFileGenerator:
    """
    Class to create files from an achievement CSV
    """

    def __init__(self) -> None:
        """Constructor"""

        self.file: str
        self.id: str

    def run(self) -> None:
        """
        Run the main functions of the class
        """

        self.parse_args()
        self.create_files()
        print("Completed successfully!")

    def parse_args(self) -> None:
        """
        Parse the arguments to the script
        """

        parser = argparse.ArgumentParser()
        parser.add_argument("file", help="Achievement CSV file")
        parser.add_argument(
            "id", help="Achievement name identifier prefix (COMPLETIONIST, etc.)"
        )
        args = parser.parse_args()
        self.file = args.file
        self.id = args.id

    def create_files(self) -> None:
        """
        Create Terraria localization and RetroAchievements mod integration files
        """

        with open(self.file, newline="", encoding="utf-8") as f:
            data_in = csv.reader(f)
            data_out = []

            with open("en-US.hjson", "w", encoding="utf-8") as f:
                f.write("Achievements: {\n")
                for line in data_in:
                    if len(line) == 4 and line[0] != "Name":
                        ra = {
                            "Title": line[1],
                            "Description": line[2],
                            "Points": int(line[3]),
                            "Type": "",
                            "Category": 5,
                            "Id": 0,
                            "Badge": "00000",
                            "Subset": "Completionist",
                        }
                        ach = {"Name": line[0], "Category": "Collector", "Ra": ra}
                        data_out.append(ach)
                        f.write(f"\t{self.id}_{line[0]}_Name: {line[1]}\n")
                        f.write(f"\t{self.id}_{line[0]}_Description: {line[2]}\n")
                f.write("}")

            with open("achievements.json", "w", encoding="utf-8") as f:
                json.dump(data_out, f, indent=4)


def main() -> None:
    """Main function"""

    AchievementFileGenerator().run()


if __name__ == "__main__":
    main()
