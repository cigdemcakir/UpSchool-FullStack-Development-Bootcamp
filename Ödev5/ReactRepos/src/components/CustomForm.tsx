import { useState } from 'react';

// library imports
import { PlusIcon } from '@heroicons/react/solid';


interface CustomFormProps {
    addTask: (task: Task) => void;
}

interface Task {
    name: string;
    checked: boolean;
    id: number;
}

const CustomForm: React.FC<CustomFormProps> = ({ addTask }) => {
    const [task, setTask] = useState("");

    const handleFormSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        addTask({
            name: task,
            checked: false,
            id: Date.now()
        });
        setTask("");
    }

    return (
        <form
            className="todo"
            onSubmit={handleFormSubmit}
        >
            <div className="wrapper">
                <input
                    type="text"
                    id="task"
                    className="input"
                    value={task}
                    onInput={(e) => setTask((e.target as HTMLInputElement).value)}
                    required
                    autoFocus
                    maxLength={60}
                    placeholder="Enter Task"
                />
                <label
                    htmlFor="task"
                    className="label"
                >Enter Task</label>
            </div>
            <button
                className="btn"
                aria-label="Add Task"
                type="submit"
            >
                <PlusIcon />
            </button>
        </form>
    )
}

export default CustomForm;
