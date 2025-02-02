"""
Converts an achievement CSV to Terraria localization and RetroAchievements mod integration files

CSV format: Name, Category, Title, Description, Points, Type, Category, Id, Badge, Set

Linted with mypy and pylint
Auto-formatted with black
"""

import argparse
import csv
import json

from dataclasses import dataclass
from typing import Any


@dataclass
class Ra:
    """
    Class to store RA-related information
    """

    # pylint: disable=too-many-instance-attributes

    title: str
    desc: str
    points: int
    rtype: str
    category: int
    id: int
    badge: str
    set: int

    def __init__(self, row: list[str]):
        """Constructor"""

        self.title = row[2]
        self.desc = row[3]
        self.points = int(row[4])
        self.rtype = row[5]
        self.category = int(row[6])

        try:
            self.id = int(row[7])
        except ValueError:
            self.id = 0

        self.badge = row[8]
        if not self.badge:
            self.badge = "00000"

        try:
            self.set = int(row[9])
        except ValueError:
            self.set = 0

    def out(self) -> dict[str, Any]:
        """
        Return output data for a JSON file
        """

        return {
            "Title": self.title,
            "Description": self.desc,
            "Points": self.points,
            "Type": self.rtype,
            "Category": self.category,
            "Id": self.id,
            "Badge": self.badge,
            "Set": self.set,
        }


class AchievementFileGenerator:
    """
    Class to create files from an achievement CSV
    """

    def __init__(self) -> None:
        """Constructor"""

        self.file: str
        self.id: str
        self.set: str

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
                for row in data_in:

                    name = row[0]
                    if name != "Name":
                        category = row[1]
                        ra = Ra(row)

                        ach = {"Name": name, "Category": category, "Ra": ra.out()}
                        data_out.append(ach)
                        f.write(f"\t{self.id}_{name}_Name: {ra.title}\n")
                        f.write(f"\t{self.id}_{name}_Description: {ra.desc}\n")

                f.write("}")

            with open("achievements.json", "w", encoding="utf-8") as f:
                json.dump(data_out, f, indent=4)


def main() -> None:
    """Main function"""

    AchievementFileGenerator().run()


if __name__ == "__main__":
    main()
