import todoLogo from './assets/todoLogo.svg';
import { useState } from 'react';
import { v4 as uuidv4 } from 'uuid';

import useLocalStorage from './hooks/useLocalStorage';

import { SortAscendingIcon, SortDescendingIcon} from '@heroicons/react/solid';

import CustomForm from './components/CustomForm';
import EditForm from './components/EditForm';
import TaskList from './components/TaskList';

function App() {
    const [tasks, setTasks] = useLocalStorage('react-todo.tasks', []);
    const [previousTask, setPreviousTask] = useState<HTMLElement | null>(null);
    const [editedTask, setEditedTask] = useState<any>(null);
    const [isEditing, setIsEditing] = useState(false);
    const [sortByCreationDate, setSortByCreationDate] = useState(false);


    const addTask = (task: any) => {
        const date = new Date();
        const dateString = date.toLocaleString();
        localStorage.setItem("createdTime", dateString);
        const newTask = { id: uuidv4(), checked: false, name: task.name, time:dateString };
        setTasks((prevState: any) => [...prevState, newTask]);
    };

    const deleteTask = (id: number) => {
        setTasks((prevState: any) => prevState.filter((t: any) => t.id !== id));
    };

    const toggleTask = (id: number) => {
        setTasks((prevState: any) =>
            prevState.map((t: any) =>
                t.id === id ? { ...t, checked: !t.checked } : t
            )
        );
    };

    const updateTask = (task: any) => {
        setTasks((prevState: any) =>
            prevState.map((t: any) =>
                t.id === task.id ? { ...t, name: task.name } : t
            )
        );
        closeEditMode();
    };

    const closeEditMode = () => {
        setIsEditing(false);
        if (previousTask) {
            previousTask.focus();
        }
    };

    const enterEditMode = (task: any) => {
        setEditedTask(task);
        setIsEditing(true);
        setPreviousTask(document.activeElement as HTMLElement);
    };

    const toggleSortByCreationDate = () => {
        setSortByCreationDate(!sortByCreationDate);
    };

    const sortedTasks = sortByCreationDate
        ? [...tasks].sort((a, b) => {
            const dateA = new Date(a.time);
            const dateB = new Date(b.time);
            return dateB.getTime() - dateA.getTime();
        })
        : tasks;


    return (
        <div className="container" >
            <header>
                <img src={ todoLogo } alt="Todo Logo"/>
            </header>
            {isEditing && (
                <EditForm
                    editedTask={editedTask}
                    updateTask={updateTask}
                    closeEditMode={closeEditMode}
                />
            )}
            <CustomForm addTask={addTask} />
            <div className="sort-button-container">
                <button className="sort-button" onClick={toggleSortByCreationDate}>
                    {sortByCreationDate ? (
                        <>
                            <SortAscendingIcon className="sort-button-icon" />
                            <span className="sort-button-text" >Sort by Oldest</span>
                        </>
                    ) : (
                        <>
                            <SortDescendingIcon className="sort-button-icon" />
                            <span className="sort-button-text">Sort by Newest</span>
                        </>
                    )}
                </button>
            </div>
            {sortedTasks && (
                <TaskList
                    tasks={sortedTasks}
                    deleteTask={deleteTask}
                    toggleTask={toggleTask}
                    enterEditMode={enterEditMode}
                />
            )}

        </div>

    );
}

export default App;
