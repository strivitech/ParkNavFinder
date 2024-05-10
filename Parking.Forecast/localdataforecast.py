import pandas as pd
import os
from prophet import Prophet
from prophet.plot import plot_plotly

# Specify the local directory where JSON files are stored
local_directory = 'E:\\Projects\\LocalData\\ParkNavFinder\\ParkingStatesData'

# List all JSON files in the directory
files = [os.path.join(local_directory, file) for file in os.listdir(local_directory) if file.endswith('.json')]

# Load data from each JSON file
data_frames = []
for file_path in files:
    df = pd.read_json(file_path, convert_dates=['CalculatedAtUtc'])
    data_frames.append(df)

# Combine all DataFrames into a single DataFrame
combined_df = pd.concat(data_frames, ignore_index=True)
combined_df['CalculatedAtUtc'] = pd.to_datetime(combined_df['CalculatedAtUtc']).dt.tz_localize(None)

# Separate forecasts for each parking lot
forecasts = {}
for parking_id, group_df in combined_df.groupby('ParkingId'):
    df = group_df[['CalculatedAtUtc', 'TotalObservers']]
    df.columns = ['ds', 'y']  # Rename columns for Prophet

    model = Prophet()
    model.fit(df)

    # Create future DataFrame for predictions
    future = model.make_future_dataframe(periods=7*24*12, freq='5T', include_history=True)

    # Predict
    forecast = model.predict(future)
    forecasts[parking_id] = forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']]

    # Print the forecast
    print(f"Forecast for Parking ID {parking_id}:")
    print(forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']])

    # Optionally, plot the forecast (commented out if running in a non-interactive environment)
    fig = plot_plotly(model, forecast)

    # Add annotation for the parking ID
    fig.add_annotation(
        x=0.5,
        y=1.05,
        xref='paper',
        yref='paper',
        text=f'Parking ID: {parking_id}',
        showarrow=False,
        font=dict(size=12, color="black"),
    )

    fig.show()
