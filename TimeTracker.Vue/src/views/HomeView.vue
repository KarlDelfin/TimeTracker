<template>
  <el-card>
    <el-container>
      <el-container>
        <el-aside class="sideMenu">
          <div class="d-flex justify-content-center p-2">
            <el-button @click="this.$router.push('/calendar')">Manage Calendar</el-button>
          </div>
          <!-- VCALENDAR -->
          <VCalendar
            :key="pickerKey"
            style="width: 100%"
            v-model="today"
            @dayclick="dayClick"
            @did-move="didMove"
            :attributes="attributes"
          />
          <!-- START CALENDAR -->
          <div class="row p-2">
            <div class="col-9">
              <el-select
                v-model="form.calendarId"
                @change="getCalendarEvent"
                placeholder="Calendar"
              >
                <el-option-group v-if="calendars.length > 0" label="My Calendars">
                  <el-option
                    v-for="calendar in calendars"
                    :key="calendar.calendarId"
                    :label="calendar.calendarName"
                    :value="calendar.calendarId"
                  />
                </el-option-group>
                <el-option-group v-if="sharedCalendars.length > 0" label="Shared Calendars">
                  <el-option
                    v-for="sharedCalendar in sharedCalendars"
                    :key="sharedCalendar.calendarId"
                    :label="sharedCalendar.calendarName"
                    :value="sharedCalendar.calendarId"
                  />
                </el-option-group>
              </el-select>
            </div>
            <div class="col-1">
              <el-button type="primary" circle @click="openForm('Create Calendar')">+</el-button>
            </div>
          </div>
          <div class="d-flex justify-content-center">
            <b>Events for {{ formatMonth(currentMonth) }}</b>
          </div>
          <!-- EVENTS -->
          <div class="row checkEvents p-2 border">
            <el-checkbox
              v-model="selectedCalendarEvent[calendarEvent.calendarEventId]"
              v-for="calendarEvent in calendarEvents"
              :key="calendarEvent.calendarEventId"
              :label="calendarEvent.eventName"
              :value="calendarEvent.calendarEventId"
              @change="handleCheckboxChange(calendarEvent.calendarEventId, calendarEvent.eventName)"
              :checked="true"
            />
          </div>
          <!-- SELECT ALL / SELECT NONE -->
          <div class="row p-2">
            <div class="col">
              <el-button class="selectAllNone" size="small" @click="checkAllEvents"
                >Select All</el-button
              >
            </div>
            <div class="col">
              <el-button class="selectAllNone" size="small" @click="uncheckAllEvents"
                >Select None</el-button
              >
            </div>
          </div>
        </el-aside>
        <el-main class="main">
          <div class="shadow" style="min-height: 80vh" v-if="isErrorFullCalendar">
            <el-empty>
              <el-button type="primary" @click="(getCalendarByUserId(), getCalendarEvent())"
                >Refresh</el-button
              >
            </el-empty>
          </div>
          <div v-else>
            <FullCalendar class="fullCalendar" ref="refCalendar" :options="calendarOptions" />
          </div>
        </el-main>
      </el-container>
    </el-container>

    <!-- START RECURRING EVENT CONFIMATION DIALOG -->
    <el-dialog
      v-model="dialog.recurringEvent"
      title="Open Recurring Event"
      width="500"
      center
      :before-close="clear"
    >
      <div class="d-flex justify-content-center mt-3">
        <span> This is one activity in a series. What do you want to open? </span>
      </div>
      <!-- START RADIO BUTTON -->
      <div class="pt-3 d-flex justify-content-center text-center text-sm">
        <el-radio-group v-model="form.isRecurring" class="ml-4">
          <el-radio label="Just this one" :value="false" size="large" />
          <el-radio label="The entire series" :value="true" size="large" />
        </el-radio-group>
      </div>
      <!-- END RADIO BUTTON -->

      <template #footer>
        <div class="dialog-footer d-flex justify-content-end">
          <el-button type="primary" @click="openForm('Open Recurring Calendar Event')">
            Confirm
          </el-button>
        </div>
      </template>
    </el-dialog>
    <!-- END RECURRING EVENT CONFIMATION DIALOG -->

    <!-- START OPERATION -->
    <el-dialog
      :before-close="clear"
      v-model="dialog.operation"
      title="Event Details"
      width="500"
      center
    >
      <div class="border p-3" style="border-radius: 20px">
        <label class="fw-bold">Event Name: </label>
        {{ calendarEvent.eventName }}
        <br />
        <label class="fw-bold mt-3">Description: </label>
        {{ calendarEvent.eventDescription }}
        <br />
        <label class="fw-bold mt-3">Date/Time Start: </label>
        {{ formatDateTime(calendarEvent.dateTimeStarted) }}
        <br />
        <label class="fw-bold mt-3">Date/Time End: </label>
        {{ formatDateTime(calendarEvent.dateTimeEnded) }}
      </div>

      <template v-if="isCalendarOwner" #footer>
        <div class="dialog-footer d-flex justify-content-end">
          <el-button @click="openForm('Open Calendar Event')"> Edit </el-button>
          <el-button type="danger" @click="deleteCalendarEvent()"> Delete </el-button>
        </div>
      </template>
    </el-dialog>
    <!-- END OPERATION -->

    <!-- START CALENDAR EVENT FORM -->
    <CalendarEventForm
      :calendarEvent="calendarEvent"
      :calendars="calendars"
      :title="title"
      :openForm="dialog.calendarEventForm"
      @close="dialog.calendarEventForm = false"
      :handleDateRange="form.dateRange"
      @refresh="(getCalendarEvent(), (dialog.operation = false))"
      :selectedDayList="selectedDays"
      :daysList="daysList"
    />
    <!-- END CALENDAR EVENT FORM -->

    <!-- START CALENDAR FORM -->
    <CalendarForm
      :title="title"
      :openForm="dialog.calendarForm"
      @close="dialog.calendarForm = false"
      @refresh="getCalendarByUserId"
    />
    <!-- END CALENDAR FORM -->
  </el-card>
</template>
<script>
import axios from 'axios'
import FullCalendar from '@fullcalendar/vue3'
import rrulePlugin from '@fullcalendar/rrule'
import dayGridPlugin from '@fullcalendar/daygrid'
import interactionPlugin from '@fullcalendar/interaction'
import timeGridPlugin from '@fullcalendar/timegrid'
import list from '@fullcalendar/list'
import CalendarForm from '@/components/CalendarForm.vue'
import CalendarEventForm from '@/components/CalendarEventForm.vue'
// const api = import.meta.env.VITE_APP_API_URL
// const api = 'https://calendar-api-eufwfccudhaebee4.eastasia-01.azurewebsites.net/api'
import * as bootstrap from 'bootstrap'
import { ElMessage, ElLoading, ElMessageBox } from 'element-plus'
export default {
  components: {
    FullCalendar,
    CalendarForm,
    CalendarEventForm,
  },
  data() {
    return {
      isCalendarOwner: false,
      isErrorFullCalendar: false,
      calendarEvent: {},
      calendarEventId: '',
      calendarEventGroupId: '',
      weekClicked: false,
      dayClicked: false,
      selectedCalendarEvent: [],
      currentStartDateTime: '',
      currentEndDateTime: '',
      form: {
        calendarId: '',
        isRecurring: false,
      },
      calendars: [],
      sharedCalendars: [],
      calendarEvents: [],
      calendarPagination: {
        currentPage: 1,
        elementsPerPage: 10,
        totalElements: 0,
      },
      user: {},

      pickerKey: 0,

      selectedDays: [],
      daysList: [],
      days: [
        { label: 'Sunday', value: 0, checked: false },
        { label: 'Monday', value: 1, checked: false },
        { label: 'Tuesday', value: 2, checked: false },
        { label: 'Wednesday', value: 3, checked: false },
        { label: 'Thursday', value: 4, checked: false },
        { label: 'Friday', value: 5, checked: false },
        { label: 'Saturday', value: 6, checked: false },
      ],
      currentMonth: '',
      currentYear: '',
      currentDay: '',
      firstDayOfMonth: '',
      lastDayOfMonth: '',
      title: '',

      dialog: {
        calendarEventForm: false,
        calendarForm: false,
        recurringEvent: false,
        operation: false,
      },

      today: new Date(),
      calendarApi: '',

      // START FULLCALENDAR
      calendarOptions: {
        datesSet: (info) => {
          this.datesSet(info)
        },
        eventTimeFormat: {
          hour: '2-digit',
          minute: '2-digit',
          meridiem: false,
          hour12: false,
        },
        eventDidMount: (info) => {
          let startTime = info.event.extendedProps.dateTimeStarted.substr(11, 19)
          let endTime = info.event.extendedProps.dateTimeEnded.substr(11, 19)

          return new bootstrap.Popover(info.el, {
            title: info.event.title,
            placement: 'auto',
            trigger: 'hover',
            customClass: 'popoverStyle',
            content: `<span class="fst-italic">${info.event.extendedProps.description}</span>
                      <hr>
                      <div class="d-flex justify-content-center">
                        <small class='fw-bold'>${startTime} - ${endTime}</small>
                      </div>
                     `,
            html: true,
          })
        },
        views: {
          dayGridMonth: {
            dayMaxEventRows: 2,
            titleFormat: { year: 'numeric', month: 'long', day: 'numeric' },
          },

          timeGridWeek: {
            slotLabelFormat: {
              hour: '2-digit',
              minute: '2-digit',
              hour12: false, // 24-hour format
            },
            eventTimeFormat: {
              hour: '2-digit',
              minute: '2-digit',
              hour12: false, // 24-hour format
            },
          },

          timeGridDay: {
            slotLabelFormat: {
              hour: '2-digit',
              minute: '2-digit',
              hour12: false, // 24-hour format
            },
          },

          listMonth: {
            eventTimeFormat: {
              hour: '2-digit',
              minute: '2-digit',
              hour12: false, // 24-hour format
            },
          },
        },
        headerToolbar: {
          start: 'todayCustom,prevCustom,nextCustom',
          center: 'title',
          end: 'createEvent monthCustom,weekCustom,dayCustom,listCustom',
        },
        // Custom Button
        customButtons: {
          createEvent: {
            text: 'Create Event',
            click: () => {
              this.openForm('Create Event')
            },
          },
          todayCustom: {
            text: 'today',
            click: () => {
              this.todayCustom()
            },
          },
          prevCustom: {
            text: 'prev',
            click: () => {
              this.prevCustom()
            },
          },
          nextCustom: {
            text: 'next',
            click: () => {
              this.nextCustom()
            },
          },
          monthCustom: {
            text: 'month',
            click: () => {
              this.monthCustom()
            },
          },
          weekCustom: {
            text: 'week',
            click: () => {
              this.weekCustom()
            },
          },
          dayCustom: {
            text: 'day',
            click: () => {
              this.dayCustom()
            },
          },
          listCustom: {
            text: 'list',
            click: () => {
              this.listCustom()
            },
          },
        },
        height: 600,
        plugins: [dayGridPlugin, interactionPlugin, timeGridPlugin, list, rrulePlugin],
        timeZone: 'UTC',
        events: [],
        firstDay: 0, // 0 - [Mon, Tue, Wed, Thu, Fri, Sat, Sun] | 1 - [Sun, Mon, Tue, Wed, Thu, Fri, Sat]
        initialView: 'dayGridMonth', // Default fullcalendar view
        eventDrop: this.moveResizeEvent, // Drag and drop
        eventResize: this.moveResizeEvent, // Resize event
        dateClick: this.handleClick, // click single date
        selectable: true, // Set selectable to true
        editable: true, // Set editable to true
        eventResizableFromStart: true, // Set resizable to true
        droppable: true, // Set drag and drop to true

        select: this.handleDateRange, // Select multiple event
        eventClick: this.eventClick, // Click event
        allDaySlot: false, // Show/hide non-recurring events false
        eventLongPressDelay: 200, // Mobile drog n drop events
        eventOverlap: true,
        forceEventDuration: true,
        displayEventTime: true, // Display time for recurring event
        showNonCurrentDates: false, // Disable last week of previous month and first week of next month
      },
    }
  },
  computed: {
    attributes() {
      return [
        // {
        //   dot: 'bar',
        //   dates: this.calendarOptions.events,
        // },
        {
          highlight: {
            start: { fillMode: 'outline' },
            base: { fillMode: 'light' },
            end: { fillMode: 'outline' },
          },
          dates: { start: new Date(this.firstDayOfMonth), end: new Date(this.lastDayOfMonth) },
        },
      ]
    },
  },
  methods: {
    createEvent() {
      this.title = 'Create Event'
      this.dialog.calendarEventForm = true
    },
    todayCustom() {
      this.pickerKey++
      this.today = new Date()
      this.calendarApi.today()
      this.getCalendarEvent()
    },
    prevCustom() {
      const prevButton = document.querySelector('button.vc-prev')
      if (prevButton) {
        this.pickerKey++
        this.calendarApi.prev()
        const currentDate = this.calendarApi.getDate()
        this.calendarApi.gotoDate(currentDate.toISOString())
        this.getCalendarEvent()
      }
    },
    nextCustom() {
      const nextButton = document.querySelector('button.vc-next')
      if (nextButton) {
        this.pickerKey++
        this.calendarApi.next()
        const currentDate = this.calendarApi.getDate()
        this.calendarApi.gotoDate(currentDate.toISOString())
        this.getCalendarEvent()
      }
    },
    monthCustom() {
      this.weekClicked = false
      this.dayClicked = false
      this.calendarApi.changeView('dayGridMonth')
      this.getCalendarEvent()
    },
    weekCustom() {
      this.weekClicked = true
      this.dayClicked = false
      this.calendarApi.changeView('timeGridWeek')
      this.getCalendarEvent()
    },
    dayCustom() {
      this.weekClicked = false
      this.dayClicked = true
      this.calendarApi.changeView('timeGridDay')
      this.getCalendarEvent()
    },
    listCustom() {
      this.calendarApi.changeView('listMonth')
      this.getCalendarEvent()
    },
    datesSet(info) {
      this.currentStartDateTime = info.view.activeStart.toISOString()
      this.currentEndDateTime = info.view.activeEnd.toISOString()
      let lastDay = new Date(info.view.activeEnd)
      lastDay.setDate(lastDay.getDate() - 1)
      this.firstDayOfMonth = info.view.activeStart.toISOString()
      this.lastDayOfMonth = lastDay.toISOString()
      this.currentMonth = info.view.activeStart.toISOString().substr(5, 2)
      this.currentYear = info.view.activeStart.toISOString().substr(0, 4)
    },
    didMove(info) {
      this.calendarApi.gotoDate(`${info[0].id}-01T00:00:00`)
      this.getCalendarEvent()
    },
    dayClick(info) {
      this.weekClicked = false
      this.dayClicked = true
      this.calendarApi.changeView('timeGridDay')
      this.calendarApi.gotoDate(info.endDate)
      this.getCalendarEvent()
    },
    // CHECK ALL CALENDAR EVENTS
    checkAllEvents() {
      this.calendarEvents.forEach((calendarEvent) => {
        this.selectedCalendarEvent[calendarEvent.calendarEventId] = true
      })

      this.getCalendarEvent()
    },

    // UNCHECK ALL CALENDAR EVENTS
    uncheckAllEvents() {
      this.calendarEvents.forEach((calendarEvent) => {
        this.selectedCalendarEvent[calendarEvent.calendarEventId] = false
      })

      this.calendarOptions.events = this.calendarOptions.events.map((event) => {
        return {
          ...event,
          display: 'none',
          backgroundColor: event.backgroundColor,
        }
      })
    },
    // CHECK SHOW/HIDE INDIVIDUAL CALENDAR EVENT
    handleCheckboxChange(calendarEventId, eventName) {
      const isChecked = this.selectedCalendarEvent[calendarEventId]

      this.calendarOptions.events = this.calendarOptions.events.map((event) => {
        if (event.title === eventName) {
          const displayStatus = isChecked ? (event.rrule ? 'list-item' : 'block') : 'none'

          return {
            ...event,
            display: displayStatus,
            backgroundColor: event.backgroundColor,
          }
        }
        return event
      })
    },
    openForm(title) {
      if (title == 'Create Calendar') {
        this.title = title
        this.dialog.calendarForm = true
        return
      }
      if (title == 'Create Event') {
        this.title = title
        this.dialog.calendarEventForm = true
        return
      }
      if (title == 'Open Calendar Event') {
        // NON-RECURRING
        if (this.calendarEventGroupId == null) {
          this.dialog.calendarEventForm = true
          this.title = 'Edit Non-Recurring Event'
        }
        // JUST THIS ONE (RECURRING)
        else if (this.calendarEventGroupId != null && this.form.isRecurring == false) {
          this.title = 'Edit Recurring Event'
          this.dialog.calendarEventForm = true
          axios.get(`${this.api}/CalendarEvent/${this.calendarEventId}`).then((response) => {
            this.calendarEvent = {
              status: 'Just this one',
              calendarEventGroupId: response.data.calendarEventGroupId,
              calendarEventId: response.data.calendarEventId,
              calendarId: response.data.calendarId,
              eventName: response.data.eventName,
              eventDescription: response.data.eventDescription,
              eventColor: response.data.eventColor,
              dateTimeStarted: `${response.data.dateTimeStarted}`,
              dateTimeEnded: `${response.data.dateTimeEnded}`,
              startTime: `${response.data.dateTimeStarted}`,
              endTime: `${response.data.dateTimeEnded}`,
              isRecurring: true,
            }
          })
        }
        // ENTIRE SERIES (RECURRING)
        else if (this.calendarEventGroupId != null && this.form.isRecurring == true) {
          this.title = 'Edit Recurring Event'
          this.dialog.calendarEventForm = true
          axios
            .get(`${this.api}/CalendarEvent/${this.calendarEventGroupId}/Group`)
            .then((response) => {
              let dateStarted = response.data[0].dateTimeStarted.substr(0, 10)
              let timeStarted = response.data[0].dateTimeStarted.substr(11, 19)
              let dateEnded = response.data[response.data.length - 1].dateTimeEnded.substr(0, 10)
              let timeEnded = response.data[response.data.length - 1].dateTimeEnded.substr(11, 19)

              this.calendarEvent = {
                status: 'series',
                calendarEventGroupId: response.data[0].calendarEventGroupId,
                calendarEventId: response.data[0].calendarEventId,
                calendarId: response.data[0].calendarId,
                eventName: response.data[0].eventName,
                eventDescription: response.data[0].eventDescription,
                eventColor: response.data[0].eventColor,
                dateTimeStarted: `${dateStarted}T${timeStarted}`,
                dateTimeEnded: `${dateEnded}T${timeEnded}`,
                startTime: `${timeStarted}`,
                endTime: `${timeEnded}`,
                isRecurring: true,
              }
              // START RECURRING EVERY
              const checkedDays = [false, false, false, false, false, false, false]
              const selectedDays = []

              response.data.forEach((item) => {
                const dayOfWeek = new Date(item.dateTimeStarted).getDay()
                if (!selectedDays.includes(dayOfWeek)) {
                  selectedDays.push(dayOfWeek)
                }
                checkedDays[dayOfWeek] = true
              })

              this.days.forEach((day) => {
                day.checked = selectedDays.includes(day.value)
              })
              this.daysList = this.days
              this.selectedDays = selectedDays
              // END RECURRING EVERY
            })
        }
        return
      }
      if (title == 'Open Recurring Calendar Event') {
        // JUST THIS ONE
        if (this.form.isRecurring == false) {
          this.dialog.operation = true
          this.title = 'Edit Non-Recurring Event'
          axios.get(`${this.api}/CalendarEvent/${this.calendarEventId}`).then((response) => {
            this.calendarEvent = {
              status: 'one',
              calendarEventGroupId: response.data.calendarEventGroupId,
              calendarEventId: response.data.calendarEventId,
              calendarId: response.data.calendarId,
              eventName: response.data.eventName,
              eventDescription: response.data.eventDescription,
              eventColor: response.data.eventColor,
              dateTimeStarted: `${response.data.dateTimeStarted}`,
              dateTimeEnded: `${response.data.dateTimeEnded}`,
              startTime: `${response.data.dateTimeStarted}`,
              endTime: `${response.data.dateTimeEnded}`,
              isRecurring: true,
            }
          })
          return
        }
        // ENTIRE SERIES
        else {
          this.dialog.operation = true
          axios
            .get(`${this.api}/CalendarEvent/${this.calendarEventGroupId}/Group`)
            .then((response) => {
              let dateStarted = response.data[0].dateTimeStarted.substr(0, 10)
              let timeStarted = response.data[0].dateTimeStarted.substr(11, 19)
              let dateEnded = response.data[response.data.length - 1].dateTimeEnded.substr(0, 10)
              let timeEnded = response.data[response.data.length - 1].dateTimeEnded.substr(11, 19)

              this.calendarEvent = {
                status: 'series',
                calendarEventGroupId: response.data[0].calendarEventGroupId,
                calendarEventId: response.data[0].calendarEventId,
                calendarId: response.data[0].calendarId,
                eventName: response.data[0].eventName,
                eventDescription: response.data[0].eventDescription,
                eventColor: response.data[0].eventColor,
                dateTimeStarted: `${dateStarted}T${timeStarted}`,
                dateTimeEnded: `${dateEnded}T${timeEnded}`,
                startTime: `${timeStarted}`,
                endTime: `${timeEnded}`,
                isRecurring: true,
              }
            })
        }
        return
      }
    },
    formatMonth(month) {
      let currentMonth = ''
      const currentDate = new Date()
      const currentDateISO = currentDate.toISOString()
      const year = currentDateISO.slice(0, 4)
      const months = [
        { id: 1, label: 'January' },
        { id: 2, label: 'February' },
        { id: 3, label: 'March' },
        { id: 4, label: 'April' },
        { id: 5, label: 'May' },
        { id: 6, label: 'June' },
        { id: 7, label: 'July' },
        { id: 8, label: 'August' },
        { id: 9, label: 'September' },
        { id: 10, label: 'October' },
        { id: 11, label: 'November' },
        { id: 12, label: 'December' },
      ]
      for (let i = 0; i < months.length; i++) {
        if (month == months[i].id) {
          currentMonth = months[i].label
        }
      }
      return `${currentMonth} ${year}`
    },

    // CLEAR
    clear() {
      this.dialog.calendarEventForm = false
      this.dialog.recurringEvent = false
      this.dialog.operation = false
      this.form.isRecurring = false
      this.calendarEventId = ''
      this.calendarEventGroupId = ''
      this.selectedDays = []
      this.isDatePickerClicked = false
      this.calendarEvent = {
        calendarEventGroupId: '',
        calendarEventId: '',
        calendarId: '',
        eventName: '',
        eventDescription: '',
        eventColor: '#409EFF',
        dateTimeStarted: '',
        dateTimeEnded: '',
        startTime: '',
        endTime: '',
        isRecurring: false,
      }
      document.querySelectorAll('.popover').forEach((data) => {
        data.style.visibility = 'hidden'
      })
    },

    // HANDLE DATE RANGE
    handleDateRange(info) {
      if (!this.isCalendarOwner) {
        this.getCalendarEvent()
        ElMessage.warning('Only CALENDAR OWNER can edit this calendar')
        return
      } else {
        let dateEnded = new Date(info.endStr)
        dateEnded.setDate(dateEnded.getDate() - 1)
        this.title = 'Create Event'
        this.dialog.calendarEventForm = true
        this.form.dateRange = {
          weekClicked: this.weekClicked,
          dayClicked: this.dayClicked,
          calendarId: this.form.calendarId,
          start: info.startStr,
          end: dateEnded,
        }
      }
    },

    // GET CALENDAR BY USER ID
    getCalendarByUserId() {
      axios
        .get(
          `${this.api}/Calendar/User/${this.user.userId}?currentPage=${this.calendarPagination.currentPage}&elementsPerPage=${this.calendarPagination.elementsPerPage}`,
        )
        .then((response) => {
          this.calendars = response.data.results
          if (this.user.userId == response.data.results[0].userId) {
            this.isCalendarOwner = true
          } else {
            this.isCalendarOwner = false
          }
          this.form.calendarId = response.data.results[0].calendarId
          this.calendarPagination.totalElements = response.data.totalElements
        })
    },

    getSharedCalendarByUserId() {
      axios
        .get(
          `${this.api}/SharedCalendar/User/${this.user.userId}?currentPage=${this.calendarPagination.currentPage}&elementsPerPage=${this.calendarPagination.elementsPerPage}`,
        )
        .then((response) => {
          if (this.user.userId == response.data.results[0].ownerUserId) {
            this.isCalendarOwner = true
          } else {
            this.isCalendarOwner = false
          }
          this.sharedCalendars = response.data.results
        })
    },

    // GET CALENDAR EVENT
    getCalendarEvent() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      setTimeout(() => {
        axios
          .get(
            `${this.api}/CalendarEvent/Calendar/${this.form.calendarId}?dateTimeStarted=${this.currentStartDateTime}&dateTimeEnded=${this.currentEndDateTime}`,
          )
          .then((response) => {
            this.calendarEvents = response.data.filter(
              (value, index, self) =>
                index === self.findIndex((event) => event.eventName === value.eventName),
            )
            this.calendarOptions.events = response.data.map((event) => {
              // event.dateTimeEnded.setDate(event.dateTimeEnded.getDate() + 1)
              return {
                id: event.calendarEventId,
                title: event.eventName,
                start: event.dateTimeStarted,
                end: event.dateTimeEnded,
                description: event.eventDescription,
                backgroundColor: event.eventColor,
                startTime: event.dateTimeStarted,
                endTime: event.dateTimeEnded,
                dateTimeEnded: event.dateTimeEnded, // use in extendedProps
                dateTimeStarted: event.dateTimeStarted, // use in extendedProps
                calendarEventGroupId: event.calendarEventGroupId, // use in extendedProps
                userId: event.userId, // use in extendedProps

                allDay:
                  this.weekClicked || this.dayClicked
                    ? false
                    : event.calendarEventGroupId == null
                      ? true
                      : false,

                rrule:
                  event.calendarEventGroupId != null
                    ? {
                        freq: 'weekly',
                        interval: 5,
                        dtstart: event.dateTimeStarted,
                        until: event.dateTimeEnded,
                      }
                    : undefined,
                display: event.calendarEventGroupId != null ? 'list-item' : 'block',
              }
            })
            loading.close()
            this.clear()
            this.dialog.operation = false
            this.isErrorFullCalendar = false
          })
          .catch(() => {
            this.isErrorFullCalendar = true
            loading.close()
          })
      }, 1000)
    },
    // EVENT CLICK
    eventClick(info) {
      // if (this.user.userId != info.event.extendedProps.userId) {
      // } else {
      // }

      this.calendarEventId = info.event.id
      this.calendarEventGroupId = info.event.extendedProps.calendarEventGroupId
      // NON-RECURRING
      if (info.event.extendedProps.calendarEventGroupId == null) {
        this.dialog.operation = true
        axios.get(`${this.api}/CalendarEvent/${this.calendarEventId}`).then((response) => {
          this.calendarEvent = {
            calendarEventGroupId: response.data.calendarEventGroupId,
            calendarEventId: response.data.calendarEventId,
            calendarId: response.data.calendarId,
            eventName: response.data.eventName,
            eventDescription: response.data.eventDescription,
            eventColor: response.data.eventColor,
            dateTimeStarted: response.data.dateTimeStarted,
            dateTimeEnded: response.data.dateTimeEnded,
            startTime: response.data.dateTimeStarted.substr(11, 19),
            endTime: response.data.dateTimeEnded.substr(11, 19),
            isRecurring: false,
          }
        })
      }
      // RECURRING
      else {
        this.dialog.recurringEvent = true
      }
    },

    // DELETE CALENDAR EVENT
    deleteCalendarEvent() {
      // ENTIRE SERIES
      if (this.calendarEventGroupId != null && this.form.isRecurring == true) {
        ElMessageBox.confirm('Do you want to delete entire series of this event?', 'Warning', {
          confirmButtonText: 'Confirm',
          cancelButtonText: 'Cancel',
          type: 'warning',
        })
          // CONFIRM
          .then(() => {
            axios
              .delete(`${this.api}/CalendarEvent/${this.calendarEventGroupId}/Group`)
              .then(() => {
                this.dialog.operation = false
                this.getCalendarEvent()
                this.clear()
                ElMessage.success('Event deleted successfully')
              })
          })
          // CANCEL
          .catch(() => {})
        return
      }
      // JUST ONE OR NON-RECURRING
      if (
        (this.calendarEventGroupId != null && this.form.isRecurring == false) ||
        this.calendarEventGroupId == null
      ) {
        ElMessageBox.confirm('Do you want to delete this event?', 'Warning', {
          confirmButtonText: 'Confirm',
          cancelButtonText: 'Cancel',
          type: 'warning',
        })
          // CONFIRM
          .then(() => {
            axios.delete(`${this.api}/CalendarEvent/${this.calendarEventId}`).then(() => {
              this.dialog.operation = false
              this.getCalendarEvent()
              this.clear()
              ElMessage.success('Event deleted successfully')
            })
          })
          // CANCEL
          .catch(() => {})
        return
      }
    },
    // MOVE / RESIZE EVENT
    moveResizeEvent(info) {
      if (!this.isCalendarOwner) {
        this.getCalendarEvent()
        ElMessage.warning('Only CALENDAR OWNER can edit this calendar')
      } else {
        let dateTimeStarted = ''
        let dateTimeEnded = ''
        let calendarEventId = info.event.id
        // MONTH LOGIC
        if (this.weekClicked == false && this.dayClicked == false) {
          let startDate = info.event.startStr.substr(0, 10)
          let endDate = info.event.endStr.substr(0, 10)
          let startTime = info.event.extendedProps.dateTimeStarted.substr(11, 19)
          let endTime = info.event.extendedProps.dateTimeEnded.substr(11, 19)
          dateTimeStarted = `${startDate}T${startTime}`
          dateTimeEnded = `${endDate}T${endTime}`
        }
        // WEEK/DAY LOGIC
        if (this.weekClicked == true || this.dayClicked == true) {
          dateTimeStarted = info.event.startStr
          dateTimeEnded = info.event.endStr
        }

        const payload = {
          dateTimeStarted: dateTimeStarted,
          dateTimeEnded: dateTimeEnded,
        }
        ElMessageBox.confirm('Do you want to update this event?', 'Warning', {
          confirmButtonText: 'Confirm',
          cancelButtonText: 'Cancel',
          type: 'warning',
        })
          // CONFIRM
          .then(() => {
            axios
              .put(`${this.api}/CalendarEvent/${calendarEventId}`, payload)
              .then(() => {
                ElMessage.success('Event updated successfully')
                this.getCalendarByUserId()
                this.clear()
              })
              .catch((error) => {
                ElMessage.error(error)
              })
          })
          // CANCEL
          .catch(() => {
            this.getCalendarByUserId()
            this.getSharedCalendarByUserId()
          })
      }
    },
    formatDateTime(dateTime) {
      if (!dateTime) {
        return ''
      }

      const months = [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July',
        'August',
        'September',
        'October',
        'November',
        'December',
      ]

      const date = new Date(dateTime)
      const year = date.getFullYear()
      const monthIndex = date.getMonth()
      const month = months[monthIndex]
      const day = date.getDate().toString().padStart(2, '0')
      const hours = date.getHours().toString().padStart(2, '0') // 24-hour format
      const minutes = date.getMinutes().toString().padStart(2, '0')

      return `${month} ${day}, ${year} ${hours}:${minutes}`
    },
  },
  mounted() {
    this.calendarApi = this.$refs.refCalendar.getApi()
    this.user = JSON.parse(localStorage.getItem('user'))
    if (this.user == null) {
      this.$router.push('/')
    }

    this.getSharedCalendarByUserId()
    this.getCalendarByUserId()

    this.getCalendarEvent()
  },
}
</script>

<style>
.fullCalendar {
  text-decoration: none;
  color: #000000;
}
.main {
  min-height: 90vh;
}
.sideMenu {
  width: 250px;
  overflow-x: hidden;
}

.timePicker {
  width: 205px;
}

.checkEvents {
  overflow-y: auto;
  overflow-x: hidden;
  height: 200px;
}

.selectAllNone {
  width: 100px;
}

.fc-createEvent-button {
  background-color: #409eff !important;
  border: 1px #409eff solid !important;
}
</style>
