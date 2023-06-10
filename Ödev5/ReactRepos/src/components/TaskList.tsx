import TaskItem from './TaskItem';

// styles
import styles from './TaskList.module.css';

interface Task {
    id: number;
    name: string;
    checked: boolean;
    time: string;
}

interface TaskListProps {
    tasks: Task[];
    deleteTask: (id: number) => void;
    toggleTask: (id: number) => void;
    enterEditMode: (task: Task) => void;
}

const TaskList: React.FC<TaskListProps> = ({ tasks, deleteTask, toggleTask, enterEditMode }) => {
    const tasksQuantity = tasks.length;
    const completedTasks = tasks.filter(task => task.checked).length;


    return (
        <section className={styles.tasks}>
            <header className={styles.header}>
                <div>
                    <p>Created tasks</p>
                    <span>{tasksQuantity}</span>
                </div>

                <div>
                    <p className={styles.textPurple}>Completed tasks</p>
                    <span>{completedTasks} of {tasksQuantity}</span>
                </div>
            </header>
        <ul className={styles.tasks}>
            {tasks.sort((a, b) => b.id - a.id).map((task: Task) => (
                <TaskItem
                    key={task.id}
                    task={task}
                    deleteTask={deleteTask}
                    toggleTask={toggleTask}
                    enterEditMode={enterEditMode}
                />
            ))}
        </ul>
        </section>
    );
};

export default TaskList;
