# -*- coding: utf-8 -*-
import random

def process_file(input_file, output_file):
    seen_lines = set()

    with open(input_file, 'r') as f:
        lines = f.readlines()

    processed_lines = []
    for line in lines:
        data_after_comma = line.strip().split(',')[1]
        if data_after_comma not in seen_lines:
            seen_lines.add(data_after_comma)
            processed_lines.append(data_after_comma)
    # processed_lines.sort(key=lambda x: (len(x), x))

    with open(output_file, 'w') as f:
        f.write('\n'.join(processed_lines))
    
    return len(processed_lines)

def random_sort(input_file, output_file, interval=72):
    with open(input_file, 'r') as f:
        lines = f.readlines()

    for i in range(0, len(lines), interval):
        batch = lines[i:i+interval]
        random.shuffle(batch)
        lines[i:i+interval] = batch

    with open(output_file, 'w') as f:
        f.writelines(lines)
    print(f"Random sort {len(lines)} lines from {input_file} to {output_file}")


if __name__ == "__main__":
    file_path = "D:/_xuan/UserStudy1/StudyTask"
    for i in range (6, 17):
        userName = "P" + str(i)
        # userName = input("Enter the user name: ")

        # for study task
        input_file_path = f"{file_path}/tasks_all_4repeat.txt" 
        output_file_path = f"{file_path}/tasks_{userName}.txt"     
        random_sort(input_file_path, output_file_path)
        
        # for practice task
        input_file_path = f"{file_path}/practice.txt"
        output_file_path = f"{file_path}/practice_{userName}.txt"
        random_sort(input_file_path, output_file_path)