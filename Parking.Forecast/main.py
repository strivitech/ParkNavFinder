import boto3
import pandas as pd
from io import BytesIO
from prophet import Prophet
from prophet.plot import plot_plotly

# Create an S3 client using default session
s3 = boto3.client('s3')
bucket_name = 'parknavfinder-parking-analytics-archival'

# List files in bucket
response = s3.list_objects_v2(Bucket=bucket_name)
files = response.get('Contents', [])

# Load data from each file
data_frames = []
for file in files:
    response = s3.get_object(Bucket=bucket_name, Key=file['Key'])
    content = response['Body'].read()
    json_str = BytesIO(content)
    df = pd.read_json(json_str, convert_dates=['CalculatedAtUtc'])
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
    future = model.make_future_dataframe(periods=30, freq='2T', include_history=True)

    # Predict
    forecast = model.predict(future)
    forecasts[parking_id] = forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']]

    # Print the forecast
    print(f"Forecast for Parking ID {parking_id}:")
    print(forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']])

    # Plot the forecast
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










# import boto3
# import pandas as pd
# from io import BytesIO
# from prophet import Prophet
# from prophet.plot import plot_plotly, plot_components_plotly
#
# # Create an S3 client using default session
# s3 = boto3.client('s3')
# bucket_name = 'parknavfinder-parking-analytics-archival'
#
# # List files in bucket
# response = s3.list_objects_v2(Bucket=bucket_name)
# files = response.get('Contents', [])
#
# # Load data from each file
# data_frames = []
# for file in files:
#     response = s3.get_object(Bucket=bucket_name, Key=file['Key'])
#     content = response['Body'].read()
#     json_str = BytesIO(content)
#     df = pd.read_json(json_str, convert_dates=['CalculatedAtUtc'])
#     data_frames.append(df)
#
# # Combine all DataFrames into a single DataFrame
# combined_df = pd.concat(data_frames, ignore_index=True)
# combined_df['CalculatedAtUtc'] = pd.to_datetime(combined_df['CalculatedAtUtc']).dt.tz_localize(None)
# df = combined_df[['CalculatedAtUtc', 'TotalObservers']]
# df.columns = ['ds', 'y']  # Rename columns for Prophet
#
# # Initialize and fit the Prophet model
# model = Prophet()
# model.fit(df)
#
# # Create future DataFrame for the next 2 minutes
# latest_timestamp = df['ds'].max()
# future = model.make_future_dataframe(periods=30, freq='2T', include_history=True)
# future['ds'] = future['ds'].apply(lambda x: x if x > latest_timestamp else latest_timestamp + pd.Timedelta(minutes=1))
#
# # Predict
# forecast = model.predict(future)
#
# # Print the forecast
# print(forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']])
#
# # Plot the forecast
# fig = plot_plotly(model, forecast)
# fig.show()
#
# # Plot forecast components
# components_fig = plot_components_plotly(model, forecast)
# components_fig.show()
#
